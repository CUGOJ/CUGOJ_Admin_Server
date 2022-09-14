namespace CUGOJ.Admin_Server.Services;

public static class AuthService
{
    public static void AddAuthMiddleWare(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;
            if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/login"))
            {
                await next();
                return;
            }
            var username = context.Request.Cookies["username"];
            var token = context.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                await HttpService.ErrorInfo(context, "token is null");
                return;
            }

            var newToken = await Dao.Dao.GetToken(username, token);
            if (newToken == null)
            {
                await HttpService.ErrorInfo(context, "token is null");
                return;
            }
            context.Response.Cookies.Append("token", newToken);
            await next();
        });
    }
}