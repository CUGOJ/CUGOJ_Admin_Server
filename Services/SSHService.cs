using Microsoft.AspNetCore.Mvc;
namespace CUGOJ.Admin_Server.Services;

public static class SSHService
{
    public record AddHostRequest
    {
        public string Name { get; set; } = null!;
        public string IP { get; set; } = null!;
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public static bool AddHost([FromBody] AddHostRequest req)
    {
        return SSH.SSH.AddHost(req.Name, req.IP, req.User, req.Password);
    }
    public static bool RemoveHost([FromBody] string name)
    {
        try
        {
            using var context = new Dao.Context();
            var host = (from h in context.Hosts where h.Name == name select h).FirstOrDefault();
            if (host == null)
                throw new Exception("未找到名为" + name + "的Host");
            context.Hosts.Remove(host);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("删除Host失败:" + e.Message);
            return false;
        }
    }
    public static void InitSSHService(WebApplication app)
    {
        app.MapPost("/addHost", AddHost);
        app.MapPost("/removeHost", RemoveHost);
    }
}