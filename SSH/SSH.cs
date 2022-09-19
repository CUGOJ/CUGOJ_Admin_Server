using Renci.SshNet;
namespace CUGOJ.Admin_Server.SSH;

public static partial class SSH
{
    public static SshClient? GetClient(string name)
    {
        try
        {
            using var context = new Dao.Context();
            var host = (from h in context.Hosts where h.Name == name select h).FirstOrDefault();
            if (host == null) return null;
            var client = new SshClient(host.HostIP, host.User, host.Password);
            client.Connect();
            return client;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public static bool AddHost(string name, string ip, string user, string password)
    {
        try
        {
            using var context = new Dao.Context();
            var host = (from h in context.Hosts where h.Name == name select h).FirstOrDefault();
            if (host != null)
            {
                throw new Exception("已存在同名Host");
            }

            using var client = new SshClient(ip, user, password);
            client.Connect();
            var result = client.RunCommand("echo \"Hello CUGOJ!\"").Result;
            if (result != "Hello CUGOJ!\n")
                throw new Exception("未能成功连接到Host");
            client.Disconnect();
            context.Hosts.Add(new Dao.Host()
            {
                Name = name,
                HostIP = ip,
                User = user,
                Password = password,
            });
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static int GetLegalPort(string hostName)
    {
        try
        {
            if (!CheckGetPortShell(hostName))
                throw new Exception("未能将脚本添加到Host");
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = "bash ~/cugoj/CUGOJ_Scripts/get-port.sh 18000 18999";
            var result = client.RunCommand(command).Result.Trim();
            return int.Parse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine("获取端口出现问题:" + e.Message);
            return 0;
        }
    }
    public static string ReDeployCore(string hostName, string env, int port, string branch)
    {
        var res = "";
        try
        {
            if (!CheckDeployCoreShell(hostName))
            {
                res = "未能将脚本添加到Host";
                throw new Exception(res);
            }
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"bash ~/cugoj/CUGOJ_Scripts/deploy-core.sh {env} {port} {branch}";
            res += client.RunCommand(command).Result.Trim();
            return res;
        }
        catch (Exception e)
        {
            Console.WriteLine("部署Core出现问题:" + e.Message);
            return "部署Core服务出现问题\n" + res;
        }
    }
    public static string DeployCore(string hostName, string env, int port, string branch)
    {
        var res = "";
        try
        {
            if (!CheckDeployCoreShell(hostName))
            {
                res = "未能将脚本添加到Host";
                throw new Exception(res);
            }
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"bash ~/cugoj/CUGOJ_Scripts/deploy-core.sh {env} {port} {branch}";
            res += client.RunCommand(command).Result.Trim();
            Dao.Dao.AddCore(env, host.Name, host.HostIP, port, branch);
            return res;
        }
        catch (Exception e)
        {
            Console.WriteLine("部署Core出现问题:" + e.Message);
            return "部署Core服务出现问题\n" + res;
        }
    }

    public static string InitMysql(string hostName, string env, int port, string password)
    {
        try
        {
            if (!CheckFileExist(hostName, "~/cugoj/CUGOJ_Scripts/mysql-cugoj.sh"))
                if (!UpdateScripts(hostName))
                    return "未能初始化脚本";
            if (!CheckFileExist(hostName, "~/cugoj/CUGOJ_Scripts/mysql-cugoj.sh"))
                return "未能初始化脚本";

            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var path = "~/cugoj/mysql/" + env;
            var command = "mkdir -p " + path + " && cd " + path + " && sh ~/cugoj/CUGOJ_Scripts/mysql-cugoj.sh " + password + " " + port.ToString() + " mysql-" + env;
            var res = "command = " + command + "\nresult = ";
            var result = client.RunCommand(command).Result;
            res += result;
            Console.WriteLine(res);
            Dao.Dao.SaveSysInfo(env, Dao.Dao.SaveSysInfoTypeEnum.Mysql, $"server={host.HostIP};port={port.ToString()};database=cugoj;user=cugoj;password={password}");
            return res;
        }
        catch (Exception e)
        {
            Console.WriteLine("初始化Mysql出现问题:" + e.Message);
            return e.Message;
        }
    }
}