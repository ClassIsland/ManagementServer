using System.Text.Json;
using System.Text.Json.Serialization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.ManagementServer.Server.Services.Grpc;
using ClassIsland.ManagementServer.Server.Services.Logging;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Pastel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();
builder.Configuration.AddJsonFile("./data/appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddDbContext<ManagementServerContext>(options =>
{
    var dbType = builder.Configuration["DatabaseType"];
    switch (dbType)
    {
        case "mysql":
            options.UseMySql(
                builder.Configuration.GetConnectionString(
        #if DEBUG
                    "Development"
        #else
                    "Production"
        #endif
                    ),ServerVersion.Parse("8.0.0-mysql"));
            break;
        default:
            throw new InvalidOperationException($"Unsupported database type: {dbType}");
    }
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
    options.DefaultSignInScheme = IdentityConstants.BearerScheme;
}).AddBearerToken();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
builder.Services.AddScoped<ObjectsUpdateNotifyService>();
builder.Services.AddScoped<ObjectsAssigneeService>();
builder.Services.AddScoped<ProfileEntitiesService>();
builder.Services.AddScoped<ClientCommandDeliverService>();
builder.Services.AddScoped<OrganizationSettingsService>();
builder.Services.AddSingleton<ObjectsCacheService>();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    
});
builder.Logging.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
builder.Services.AddIdentityApiEndpoints<User>(options =>
    {
        
    })
    .AddEntityFrameworkStores<ManagementServerContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Welcome to {}!", "ClassIsland.ManagementServer".Pastel("#00bfff"));

app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapGrpcService<ClientRegisterService>();
app.MapGrpcService<ClientCommandDeliverFrontedService>();
app.MapGrpcService<AuditService>();
app.MapGrpcService<ConfigUploadService>();

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ManagementServerContext>();
    if (app.Environment.IsDevelopment())
    {
        db.Database.Migrate();
    }

    if (config["migrate"] == "true")
    {
        db.Database.Migrate();
        return;
    }

    var migrations = await db.Database.GetPendingMigrationsAsync();
    if (migrations.Any())
    {
        logger.LogWarning("数据库未迁移，请在运行应用前先完成数据库迁移。使用参数 --migrate=true 进行数据库迁移");
        return;
    }
    await db.SetupDatabase();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    if (!await userManager.Users.AnyAsync())
    {
        await userManager.CreateAsync(new User()
        {
            Name = "root",
            UserName = "root"
        }, "ClassIslandAdmin123!");
        logger.LogInformation("已创建初始用户：用户名 root, 密码 ClassIslandAdmin123!");
    }
}

app.Run();
