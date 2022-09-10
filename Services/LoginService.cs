using System.Security.Cryptography;
namespace CUGOJ.Admin_Server.Services;

public static class LoginService
{
    public static bool Login(string username, string password)
    {
        var user = Dao.Dao.GetUser(username);
        if (user == null)
        {
            return false;
        }
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + user.Salt));
        return hash.ToString() == user.Password;
    }

    public static int Logup(string username, string password, int role)
    {
        var user = Dao.Dao.GetUser(username);
        if (user != null)
        {
            throw new Exception("用户名已存在");
        }
        using var md5 = MD5.Create();
        var salt = new Random().NextInt64().ToString();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        if (hash == null)
            throw new Exception("密码加密失败");
        Dao.Dao.SaveUsers(new Dao.User
        {
            Username = username,
            Password = System.Text.Encoding.UTF8.GetString(hash),
            Salt = salt.ToString(),
            Role = role,
        });
        return 0;
    }
}