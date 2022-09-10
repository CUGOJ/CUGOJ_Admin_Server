using CUGOJ.Admin_Server.Dao;
using CUGOJ.Admin_Server.Services;
namespace CUGOJ.Admin_Server;
public class Init
{
    public static void InitSystem()
    {
        if (!Directory.Exists("data"))
        {
            Directory.CreateDirectory("data");
        }
        // File.Create("data/data.db").Close();
        Console.WriteLine("开始初始化系统");
        try
        {
            Console.WriteLine("正在创建数据库");
            using var context = new Context();
            context.Database.EnsureCreated();
            Console.WriteLine("数据库创建成功");
            Console.WriteLine("正在初始化数据库信息");
            Dao.Dao.SaveSysInfo(new SysInfo
            {
                MysqlPath = "null",
                RedisPath = "null",
                Neo4jPath = "null",
                RabbitmqPath = "null",
                TracePath = "null",
                LogPath = "null",
            });
            LoginService.Logup("admin", "admin", 0);
            var admin = Dao.Dao.GetUser("admin");
            if (admin == null)
                throw new Exception("数据库初始化失败");
            Console.WriteLine("初始用户 username= " + admin.Username + " ,password= " + admin.Password);
            Console.WriteLine("数据库信息初始化成功");
        }
        catch (Exception e)
        {
            Console.WriteLine("初始化系统失败,发生错误:" + e.Message);
            using var context = new Context();
            context.Database.EnsureDeleted();
            throw e;
        }
    }
}