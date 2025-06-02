using System.Text.Json;
using System.Text.Json.Serialization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.ManagementServer.Server.Services.Grpc;
using ClassIsland.ManagementServer.Server.Services.Logging;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ManagementServerContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString(
#if DEBUG
            "Development"
#else
            "Production"
#endif
            ),ServerVersion.Parse("8.0.0-mysql"));
    // options.EnableSensitiveDataLogging();
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

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
builder.WebHost.ConfigureKestrel((context, options) =>
{
    
});
builder.Logging.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<ManagementServerContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

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
app.MapGroup("/api/v1/identity")
    .MapIdentityApi<User>();

app.MapFallbackToFile("/index.html");

#if DEBUG  // 处于开发环境时需要自动迁移
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ManagementServerContext>();
    db.Database.Migrate();
    await db.SetupDatabase();
}
#endif

app.Run();
