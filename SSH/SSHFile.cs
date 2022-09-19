namespace CUGOJ.Admin_Server.SSH;

public static partial class SSH
{
    private static bool CheckDictionaryExist(string hostName, string dictionaryPath)
    {
        try
        {
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"[ -d {dictionaryPath} ] && echo true || echo false";
            var result = client.RunCommand(command).Result.Trim();
            return result == "true";
        }
        catch (Exception e)
        {
            Console.WriteLine("检查文件出现问题:" + e.Message);
            return false;
        }
    }
    private static bool CheckFileExist(string hostName, string fileName)
    {
        try
        {
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"[ -f {fileName} ] && echo true || echo false";
            var result = client.RunCommand(command).Result.Trim();
            return result == "true";
        }
        catch (Exception e)
        {
            Console.WriteLine("检查文件出现问题:" + e.Message);
            return false;
        }
    }
    private static bool UpdateScripts(string hostName)
    {
        try
        {
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"mkdir -p ~/cugoj && cd ~/cugoj && git clone https://ghproxy.com/https://github.com/CUGOJ/CUGOJ_Scripts.git";
            if (CheckDictionaryExist(hostName, "~/cugoj/CUGOJ_Scripts"))
                command = $"rm -rf ~/cugoj/CUGOJ_Scripts && cd ~/cugoj && git clone https://ghproxy.com/https://github.com/CUGOJ/CUGOJ_Scripts.git";
            var result = client.RunCommand(command).Result.Trim();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("添加文件出现问题:" + e.Message);
            return false;
        }
    }
    private static bool AddFile(string hostName, string fileName, string content)
    {
        try
        {
            var host = Dao.Dao.GetHost(hostName);
            if (host == null)
                throw new Exception("未找到对应主机");
            using var client = GetClient(hostName);
            if (client == null)
                throw new Exception("未找到对应主机");
            var command = $"echo \"{content}\" > {fileName}";
            var result = client.RunCommand(command).Result.Trim();
            return result == "";
        }
        catch (Exception e)
        {
            Console.WriteLine("添加文件出现问题:" + e.Message);
            return false;
        }
    }
    private static bool CheckScript(string hostName, string scriptName)
    {
        try
        {
            if (!CheckFileExist(hostName, $"~/cugoj/CUGOJ_Scripts/{scriptName}"))
            {
                if (!UpdateScripts(hostName))
                    throw new Exception("添加文件出现问题");
            }
            if (!CheckFileExist(hostName, $"~/cugoj/CUGOJ_Scripts/{scriptName}"))
                return false;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("检查脚本出现问题:" + e.Message);
            return false;
        }
    }
    private static bool CheckGetPortShell(string hostName)
    {
        return CheckScript(hostName, "get-port.sh");
    }
    private static bool CheckDeployCoreShell(string hostName)
    {
        return CheckScript(hostName, "deploy-core.sh");
    }
}