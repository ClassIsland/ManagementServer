using System.Text.Json;
using System.Text.Json.Serialization;
using ClassIsland.ManagementServer.Server;
using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Models.Authorization;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.ManagementServer.Server.Services.Grpc;
using ClassIsland.ManagementServer.Server.Services.Logging;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Pastel;
using Spectre.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();
var setupMode = config["setup"] == "true";
var migrateMode = config["migrate"] == "true";
if (setupMode)
{
    var result = Setup.SetupPhase1(builder);
    if (!result)
    {
        return;
    }
}
builder.Configuration.AddJsonFile("./data/appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddDbContext<ManagementServerContext>(options =>
{
    var dbType = builder.Configuration["DatabaseType"];
    switch (dbType)
    {
        case "mysql":
            options.UseMySql(
                builder.Configuration.GetConnectionString(
                    builder.Environment.IsDevelopment() ? "Development" : "Production"
                ),ServerVersion.Parse("8.0.0-mysql"));
            break;
        default:
            throw new InvalidOperationException($"Unsupported database type: {dbType}");
    }
});
builder.Services.AddAuthorizationBuilder();
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
builder.Services.AddSingleton<IAuthorizationHandler, AdminAccessHandler>();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    
});
builder.Logging.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
builder.Services.AddIdentityApiEndpoints<User>(options =>
    {
        
    })
    .AddRoles<Role>()
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

    if (migrateMode || setupMode)
    {
        db.Database.Migrate();

        if (!setupMode)
        {
            logger.LogInformation("已完成数据库迁移，应用即将退出");
            return;
        }
    }

    var migrations = await db.Database.GetPendingMigrationsAsync();
    if (migrations.Any())
    {
        logger.LogWarning("数据库未迁移，请在运行应用前先完成数据库迁移。使用参数 --migrate=true 进行数据库迁移");
        return;
    }
    await db.SetupDatabase();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    foreach (var role in Roles.AllRoles.Where(x => !roleManager.Roles.Any(y => y.Id == x)).ToList())
    {
        await roleManager.CreateAsync(new Role()
        {
            Id = role,
            Name = role
        });
    }
    if (setupMode && !await userManager.Users.AnyAsync())
    {
        await Setup.SetupPhase2(userManager);
        var rootUser = await userManager.FindByNameAsync("root");
        if (rootUser != null)
        {
            foreach (var role in Roles.AllRoles)
            {
                await userManager.AddToRoleAsync(rootUser, role);
            }
        }
    }
}

if (setupMode)
{
    AnsiConsole.MarkupLine(
        $"[purple](!)[/] 恭喜！您已完成 ClassIsland.ManagementServer 的基本设置，运行 start.sh/start.ps1 即可启动服务。按任意键继续。");
    Console.ReadKey();
    return;
}

app.Run();
