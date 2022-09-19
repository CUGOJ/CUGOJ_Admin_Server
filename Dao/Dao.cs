using System.Diagnostics;
namespace CUGOJ.Admin_Server.Dao;

public static class Dao
{
    public static long SaveUsers(User user)
    {
        using var context = new Context();
        context.Users.Update(user);
        context.SaveChanges();
        return user.Id;
    }

    public static long SaveCore(Core core)
    {
        using var context = new Context();
        context.Cores.Update(core);
        context.SaveChanges();
        return core.Id;
    }

    public static long SaveAuth(Auth auth)
    {
        using var context = new Context();
        context.Auths.Update(auth);
        context.SaveChanges();
        return auth.Id;
    }

    public enum SaveSysInfoTypeEnum
    {
        Log,
        Trace,
        Mysql,
        Redis,
        Neo4j,
        RabbitMQ
    }
    public static SysInfo AddSysInfo(string env)
    {
        using var context = new Context();
        var sysInfo = new SysInfo()
        {
            Env = env,
            LogPath = string.Empty,
            TracePath = string.Empty,
            MysqlPath = string.Empty,
            RedisPath = string.Empty,
            RabbitmqPath = string.Empty,
            Neo4jPath = string.Empty
        };
        context.SysInfos.Add(sysInfo);
        context.SaveChanges();
        return sysInfo;
    }
    public static void SaveSysInfo(string env, SaveSysInfoTypeEnum type, string value)
    {
        using var context = new Context();
        var sysInfo = (from i in context.SysInfos where i.Env == env select i).FirstOrDefault();
        if (sysInfo == null)
            sysInfo = new SysInfo() { Env = env };
        switch (type)
        {
            case SaveSysInfoTypeEnum.Log: sysInfo.LogPath = value; break;
            case SaveSysInfoTypeEnum.Trace: sysInfo.TracePath = value; break;
            case SaveSysInfoTypeEnum.Mysql: sysInfo.MysqlPath = value; break;
            case SaveSysInfoTypeEnum.Redis: sysInfo.RedisPath = value; break;
            case SaveSysInfoTypeEnum.RabbitMQ: sysInfo.RabbitmqPath = value; break;
            case SaveSysInfoTypeEnum.Neo4j: sysInfo.Neo4jPath = value; break;
        }
        context.Update(sysInfo);
        context.SaveChanges();
    }

    public static void SaveSysInfo(SysInfo info)
    {
        using var context = new Context();
        context.Update(info);
        context.SaveChanges();
    }

    public static User? GetUser(string username)
    {
        using var context = new Context();
        return context.Users.FirstOrDefault(u => u.Username == username);
    }


    public static async Task<string?> GetToken(string username, string? lastToken = null)
    {
        using var context = new Context();
        string token = Guid.NewGuid().ToString();
        var user = (from u in context.Users where u.Username == username select u).FirstOrDefault();
        if (user == null) return null;
        if (lastToken != null && user.Token != lastToken) return null;
        user.Token = token;
        await context.SaveChangesAsync();
        return token;
    }

    public static Host? GetHost(string name)
    {
        try
        {
            using var context = new Context();
            return (from h in context.Hosts where h.Name == name select h).FirstOrDefault();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void AddCore(string env, string host, string ip, int port, string branch)
    {
        using var context = new Context();
        var core = new Core()
        {
            Env = env,
            Host = host,
            IP = ip,
            Port = port,
            LogPath = string.Empty,
            TracePath = string.Empty,
            MysqlPath = string.Empty,
            RedisPath = string.Empty,
            RabbitmqPath = string.Empty,
            Neo4jPath = string.Empty,
            Branch = branch
        };
        context.Cores.Add(core);
        context.SaveChanges();
    }

}