using System.Text.RegularExpressions;
using CUGOJ.Admin_Server.Dao;
namespace CUGOJ.Admin_Server.Services;
public static class SysService
{
    public static List<SysInfo> GetSysInfo()
    {
        try
        {
            using var context = new Context();
            var user = ServiceContext.User;
            var sysInfos = (from s in context.SysInfos where s.Env == user || s.Env == "public" select s).ToList();
            if (!(from s in sysInfos where s.Env == user select s).Any())
                sysInfos.Add(Dao.Dao.AddSysInfo(user));
            if (!(from s in sysInfos where s.Env == "public" select s).Any())
                sysInfos.Add(Dao.Dao.AddSysInfo("public"));

            sysInfos.ForEach(s => s.MysqlPath = Regex.Replace(s.MysqlPath, @"(password=.*)", "password=********"));
            return sysInfos;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return new List<SysInfo>();
    }

    public static List<CUGOJ.Admin_Server.Dao.Host> GetHosts()
    {
        try
        {
            using var context = new Context();
            return (from h in context.Hosts select h).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return new();
    }

    public static void InitSysService(WebApplication app)
    {
        app.MapGet("/getSysInfo", GetSysInfo);
        app.MapGet("/getHosts", GetHosts);
    }
}