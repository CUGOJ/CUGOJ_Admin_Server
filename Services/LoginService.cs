using System.Net;
using CUGOJ.Admin_Server.Dao;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
namespace CUGOJ.Admin_Server.Services;

public static class LoginService
{
    public static async Task<bool> Login(string username, string password)
    {
        return await Task.Run<bool>(() =>
        {
            var user = Dao.Dao.GetUser(username);
            if (user == null)
            {
                return false;
            }
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + user.Salt));
            return System.Text.Encoding.UTF8.GetString(hash) == user.Password;
        });
    }

    public static async Task<long> Logup(string username, string password, int role)
    {
        var user = Dao.Dao.GetUser(username);
        if (user != null)
        {
            return -1;
        }
        using var md5 = MD5.Create();
        var salt = new Random().NextInt64().ToString();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        if (hash == null)
            return -1;
        return await Dao.Dao.SaveUsers(new Dao.User
        {
            Username = username,
            Password = System.Text.Encoding.UTF8.GetString(hash),
            Salt = salt.ToString(),
            Role = role,
            Token = string.Empty
        });
    }

    public static async Task<bool> ChangePassword(string username, string oldPassword, string password)
    {
        if (!await Login(username, oldPassword))
            return false;
        var user = Dao.Dao.GetUser(username);
        if (user == null)
        {
            return false;
        }
        using var md5 = MD5.Create();
        var salt = new Random().NextInt64().ToString();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        if (hash == null)
            return false;
        return (await Dao.Dao.SaveUsers(new Dao.User
        {
            Id = user.Id,
            Username = username,
            Password = System.Text.Encoding.UTF8.GetString(hash),
            Salt = salt.ToString(),
            Role = user.Role,
            Token = user.Token
        }) != -1);
    }

    record HttpLoginStruct
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    record HttpChangePasswordStruct
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
    public static void InitLoginService(WebApplication app)
    {
        app.MapPost("/login", async (HttpContext context) =>
        {
            StreamReader reader = new StreamReader(context.Request.Body);
            string body = await reader.ReadToEndAsync();
            var loginStruct = System.Text.Json.JsonSerializer.Deserialize<HttpLoginStruct>(body);
            if (loginStruct == null)
            {
                return false;
            }
            if (await Login(loginStruct.Username, loginStruct.Password))
            {
                var token = await Dao.Dao.GetToken(loginStruct.Username);
                if (token != null)
                {
                    context.Response.Cookies.Append("token", token);
                    context.Response.Cookies.Append("username", loginStruct.Username);
                }
                return true;
            }
            else
            {
                return false;
            }
        });
        app.MapPost("/logup", async (HttpLoginStruct loginStruct) =>
        {
            return await Logup(loginStruct.Username, loginStruct.Password, 2);
        });
        app.MapPost("/changePassword", async (HttpChangePasswordStruct req) =>
        {
            return await ChangePassword(req.Username, req.OldPassword, req.NewPassword);
        });
    }

}