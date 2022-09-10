namespace CUGOJ.Admin_Server.Dao;

public static class Dao
{
    public static void SaveUsers(User user)
    {
        using var context = new Context();
        context.Users.Update(user);
        context.SaveChanges();
    }

    public static void SaveCore(Core core)
    {
        using var context = new Context();
        context.Cores.Update(core);
        context.SaveChanges();
    }

    public static void SaveAuth(Auth auth)
    {
        using var context = new Context();
        context.Auths.Update(auth);
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
}