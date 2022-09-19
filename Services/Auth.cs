namespace CUGOJ.Admin_Server.Services;

public static class AuthService
{
    public static bool Debug { get; set; } = false;
    public static void AddAuthMiddleWare(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;
            if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/login") || Debug)
            {
                await next();
                return;
            }
            var username = context.Request.Cookies["username"];
            var token = context.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                await HttpService.SuccessInfo(context, "token is null");
                return;
            }

            var newToken = await Dao.Dao.GetToken(username, token);
            if (newToken == null)
            {
                context.Response.Cookies.Append("username", "logout", new CookieOptions()
                {
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                await HttpService.SuccessInfo(context, "token is null");
                return;
            }
            context.Response.Cookies.Append("token", newToken, new CookieOptions()
            {
                Secure = true,
                SameSite = SameSiteMode.None
            });
            ServiceContext.User = username;
            await next();
        });
    }
}