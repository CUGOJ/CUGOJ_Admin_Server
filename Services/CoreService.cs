using System.Text.RegularExpressions;
using System.Diagnostics;
using CUGOJ.RPC.Gen.Services.Core;
using Thrift;
using Thrift.Transport;
using Thrift.Transport.Client;
using Thrift.Protocol;
using CUGOJ.Admin_Server.Dao;
using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.Tools;
using CUGOJ.CUGOJ_Tools.RPC;
using Microsoft.AspNetCore.Mvc;

namespace CUGOJ.Admin_Server.Services;
public static class CoreService
{
    private static async Task<CUGOJ.RPC.Gen.Services.Core.CoreService.Client?> GetClient(string env)
    {
        try
        {
            using var context = new Context();
            var coreInfo = (from c in context.Cores where c.Env == env select c).FirstOrDefault();
            if (coreInfo == null)
                throw new Exception("未知的环境");
            var ipAddress = System.Net.IPAddress.Parse(coreInfo.IP);
            TTransport transport = new TSocketTransport(ipAddress, coreInfo.Port, new TConfiguration(), 1000);
            TProtocol protocol = new TBinaryProtocol(transport);
            await transport.OpenAsync();
            var client = new CUGOJ.RPC.Gen.Services.Core.CoreService.Client(protocol);
            return client;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public static async Task<long> Ping(string env)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var startTime = CommonTools.UnixMili();
            var req = new PingRequest(startTime);
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.Ping(req);
            return resp.Timestamp - startTime;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 0;
        }
    }

    public static async Task<List<ServiceBaseInfo>> GetAllServices(string env)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var req = new GetAllServicesRequest();
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.GetAllServices(req);
            return resp.Services;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new List<ServiceBaseInfo>();
        }
    }
    public static List<Core> GetCoreList()
    {
        try
        {
            var user = ServiceContext.User;
            if (user == "")
            {
                throw new Exception("未登录");
            }
            using var context = new Context();
            var coreList = (from c in context.Cores where c.Env == user || (c.Env == "prod" && user == "admin") select c).ToList();
            coreList.ForEach(c =>
            {
                c.MysqlPath = Regex.Replace(c.MysqlPath, @"(password=.*)", "password=********");
            });
            return coreList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return new();
    }

    public record RestartRequest
    {
        public string Env { get; set; } = null!;
    }

    public static async Task<string> Restart([FromBody] RestartRequest restartRequest)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(restartRequest.Env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var req = new CUGOJ.RPC.Gen.Services.Core.RestartRequest();
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.Restart(req);
            return "服务正在重启";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "重启失败";
        }
    }

    public record UpdateServiceSetupRequest
    {
        public string Env { get; set; } = null!;
        public string ServiceID { get; set; } = null!;
        public SetupServiceTypeEnum SetupType { get; set; }
        public string Value { get; set; } = null!;
    }
    public static async Task<string> UpdateServiceSetup([FromBody] UpdateServiceSetupRequest updateServiceSetupRequest)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(updateServiceSetupRequest.Env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var req = new SetupServiceRequest(updateServiceSetupRequest.ServiceID, updateServiceSetupRequest.SetupType, updateServiceSetupRequest.Value);
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.SetupService(req);
            return "服务配置成功";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "配置失败";
        }
    }

    public static async Task<string> GetConnectionString(string env, string serviceID)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var req = new GetConnectionStringByServiceIDRequest(serviceID);
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.GetConnectionStringByServiceID(req);
            return resp.ConnectionString;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "获取失败";
        }
    }
    public record RegisterNewServiceRequest
    {
        public string Env { get; set; } = null!;
        public ServiceTypeEnum ServiceType { get; set; }
    }
    public static async Task<string> RegisterNewService([FromBody] RegisterNewServiceRequest registerNewServiceRequest)
    {
        try
        {
            using var activity = new Activity("CUGOJ.Admin_Server");
            activity.Start();
            var client = await GetClient(registerNewServiceRequest.Env);
            if (client == null)
                throw new Exception("未能成功连接到Core");
            var req = new AddServiceRequest(registerNewServiceRequest.ServiceType);
            req.Base = RPCTools.NewRootBase();
            req.Base.Extra["ServiceID"] = "CUGOJ.Admin_Server";
            req.Base.Extra["UserID"] = "admin";
            var resp = await client.AddService(req);
            return resp.ConnectionString;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "配置失败";
        }
    }

    public static void InitCoreService(WebApplication app)
    {
        app.MapGet("/getCoreList", GetCoreList);
        app.MapGet("/ping", Ping);
        app.MapGet("/getAllServices", GetAllServices);
        app.MapPost("/restart", Restart);
        app.MapPost("/updateServiceSetup", UpdateServiceSetup);
        app.MapGet("/getConnectionString", GetConnectionString);
        app.MapPost("/registerNewService", RegisterNewService);
    }
}