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