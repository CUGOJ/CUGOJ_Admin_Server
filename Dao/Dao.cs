namespace CUGOJ.Admin_Server.Dao;

public static class Dao
{
    public static async Task<long> SaveUsers(User user)
    {
        using var context = new Context();
        context.Users.Update(user);
        await context.SaveChangesAsync();
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


    public static async Task<string?> GetToken(string username, string? lastToken = null)
    {
        using var context = new Context();
        string token = Guid.NewGuid().ToString();
        var user = (from u in context.Users where u.Username == username select u).First();
        if (user == null) return null;
        if (lastToken != null && user.Token != lastToken) return null;
        user.Token = token;
        await context.SaveChangesAsync();
        return token;
    }
}