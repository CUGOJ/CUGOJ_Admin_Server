namespace CUGOJ.Admin_Server.Services;

public static class HttpService
{
    public static void InitHttpService(WebApplication app)
    {
        LoginService.InitLoginService(app);
        SSHService.InitSSHService(app);
    }


    public static async Task ErrorInfo(HttpContext context, string info)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync(info);
    }

    public static async Task SuccessInfo(HttpContext context, string info = "")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(info);
    }

}