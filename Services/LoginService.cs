using CUGOJ.Admin_Server.Dao;
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
            return hash.ToString() == user.Password;
        });
    }

    public static long Logup(string username, string password, int role)
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
        return Dao.Dao.SaveUsers(new Dao.User
        {
            Username = username,
            Password = System.Text.Encoding.UTF8.GetString(hash),
            Salt = salt.ToString(),
            Role = role,
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
        return (Dao.Dao.SaveUsers(new Dao.User
        {
            Id = user.Id,
            Username = username,
            Password = System.Text.Encoding.UTF8.GetString(hash),
            Salt = salt.ToString(),
            Role = user.Role,
        }) != -1);
    }

    public static void InitLoginService(WebApplication app)
    {
        app.MapPost("/login", (string username, string password) =>
        {
            return Login(username, password);
        });
        app.MapPost("/logup", (string username, string password) =>
        {
            return Logup(username, password, 2);
        });
        app.MapPost("/changePassword", (string username, string oldPassword, string password) =>
        {
            return ChangePassword(username, oldPassword, password);
        });
    }

}