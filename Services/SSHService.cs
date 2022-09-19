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
    public record RemoveHostRequest
    {
        public string Name { get; set; } = null!;
    }
    public static bool RemoveHost([FromBody] RemoveHostRequest req)
    {
        try
        {
            using var context = new Dao.Context();
            var host = (from h in context.Hosts where h.Name == req.Name select h).FirstOrDefault();
            if (host == null)
                throw new Exception("未找到名为" + req.Name + "的Host");
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

    public record InitMysqlRequest
    {
        public string HostName { get; set; } = string.Empty;
        public string Env { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public static string InitMysql([FromBody] InitMysqlRequest req)
    {
        var port = SSH.SSH.GetLegalPort(req.HostName);
        if (port == 0)
            return "未能在主机中找到可用端口";
        return SSH.SSH.InitMysql(req.HostName, req.Env, port, req.Password);
    }

    public record DeployCoreRequest
    {
        public string HostName { get; set; } = string.Empty;
        public string Env { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
    }
    public static string DeployCore([FromBody] DeployCoreRequest req)
    {
        var port = SSH.SSH.GetLegalPort(req.HostName);
        if (port == 0)
            return "未能在主机中找到可用端口";
        return SSH.SSH.DeployCore(req.HostName, req.Env, port, req.Branch);
    }

    public record ReDeployCoreRequest
    {
        public string Env { get; set; } = string.Empty;
    }
    public static string ReDeployCore([FromBody] ReDeployCoreRequest req)
    {
        var Core = CoreService.GetCoreList().FirstOrDefault(c => c.Env == req.Env);
        if (Core == null)
            return "未找到对应的Core";
        return SSH.SSH.ReDeployCore(Core.Host, Core.Env, Core.Port, Core.Branch);
    }

    public static void InitSSHService(WebApplication app)
    {
        app.MapPost("/addHost", AddHost);
        app.MapPost("/removeHost", RemoveHost);
        app.MapPost("/initMysql", InitMysql);
        app.MapPost("/deployCore", DeployCore);
        app.MapPost("/reDeployCore", ReDeployCore);
    }

}