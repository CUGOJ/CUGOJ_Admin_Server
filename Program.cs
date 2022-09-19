using CUGOJ.Admin_Server;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

if ((from arg in args where arg == "-debug" select arg).Any())
    CUGOJ.Admin_Server.Services.AuthService.Debug = true;

if (!File.Exists("./data/data.db"))
{
    try
    {
        Init.InitSystem();
    }
    catch (Exception e)
    {
        Console.WriteLine("系统初始化失败" + e.Message);
        return;
    }
}


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var CorsName = "admin_client";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsName, policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(CorsName);

AuthService.AddAuthMiddleWare(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

HttpService.InitHttpService(app);

app.Run();
