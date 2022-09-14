using CUGOJ.Admin_Server;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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

var app = builder.Build();
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
