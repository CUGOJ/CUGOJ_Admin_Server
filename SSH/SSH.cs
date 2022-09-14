using Renci.SshNet;
namespace CUGOJ.Admin_Server.SSH;

public static class SSH
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

    public static bool
}