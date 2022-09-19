namespace CUGOJ.Admin_Server.Services;

public static class ServiceContext
{
    private static AsyncLocal<string> _user = new AsyncLocal<string>();
    public static string User
    {
        get
        {
            return _user.Value == null ? "" : _user.Value;
        }
        set
        {
            _user.Value = value;
        }
    }
}