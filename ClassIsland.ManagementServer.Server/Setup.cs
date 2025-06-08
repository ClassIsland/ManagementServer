using System.Text.Json;
using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Pastel;
using Spectre.Console;

namespace ClassIsland.ManagementServer.Server;

public static class Setup
{
    private const string AppSettingsPath = "./data/appsettings.json";
    
    public static bool SetupPhase1(WebApplicationBuilder builder)
    {
        Console.WriteLine($"欢迎使用 {"ClassIsland.ManagementServer".Pastel("#00bfff")}！");
        Console.WriteLine($"让我们通过几个简单的步骤完成 {"ClassIsland.ManagementServer".Pastel("#00bfff")} 的设置。");
        var configPath = Path.Combine(builder.Environment.ContentRootPath, AppSettingsPath);
        if (File.Exists(configPath))
        {
            var proceed = AnsiConsole.Prompt(new TextPrompt<bool>("[yellow](!)[/] 检测到已存在的配置文件，继续本向导将覆盖此文件。您确定要继续吗？")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(false)
                .WithConverter(choice => choice ? "y" : "n"));
            if (!proceed)
            {
                return false;
            }
        }
        
        AnsiConsole.Write(new Rule("[cyan] 数据库 [/]")
        {
            Justification = Justify.Left
        });
        var wizardType = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 你要如何设置数据库？(w-向导配置/s-输入连接字符串)")
                .AddChoices(["w", "s"])
                .DefaultValue("w"));
        var finish = false;
        var connectionString = "";
        while (!finish)
        {
            connectionString = wizardType == "w" ? GetConnectionStringsByWizard() : GetConnectionStringManual();
            finish = AnsiConsole.Prompt(new TextPrompt<bool>($"[purple](*)[/] 数据库配置完成，连接字符串是 [bold]{connectionString}[/] 。配置是否正确？")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
        }
        
        AnsiConsole.Write(new Rule("[cyan] Web [/]")
        {
            Justification = Justify.Left
        });
        var hostApi = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 应用（Web UI、API 等）监听 URL")
                .DefaultValue("http://localhost:20721"));
        var hostGrpc = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] Grpc 监听 URL")
                .DefaultValue("http://localhost:20722"));

        var config = new
        {
            ConnectionStrings = new {
                Production = connectionString
            },
            Kestrel = new
            {
                Endpoints = new
                {
                    api = new
                    {
                        Url = hostApi,
                        Protocols = "Http1"
                    },
                    grpc = new
                    {
                        Url = hostGrpc,
                        Protocols = "Http2"
                    }
                }
            },
            DatabaseType = "mysql"
        };
        
        AnsiConsole.MarkupLine($"[purple](*)[/] 正在保存配置…");
        File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
        AnsiConsole.MarkupLine($"[purple](*)[/] 正在准备配置服务器…");
        AnsiConsole.Write(new Rule("[purple] 服务器初始化 [/]")
        {
            Justification = Justify.Left
        });
        return true;
    }

    public static async Task SetupPhase2(UserManager<User> userManager)
    {
        AnsiConsole.Write(new Rule("[cyan] 用户 [/]")
        {
            Justification = Justify.Left
        });

        while (true)
        {
            var rootPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan](?)[/] 超级管理员 (root) 密码")
                    .Secret());
            var confirmRootPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan](?)[/] 确认超级管理员 (root) 密码")
                    .Secret());
            if (rootPassword != confirmRootPassword)
            {
                AnsiConsole.MarkupLine("[red](!)[/] 密码不一致，请重新输入密码");
                continue;
            }
            
            var result = await userManager.CreateAsync(new User()
            {
                Name = "root",
                UserName = "root",
            }, rootPassword);

            if (result.Succeeded)
            {
                break;
            }
            
            AnsiConsole.MarkupLine($"[red](!)[/] 创建用户时发生错误：{string.Join(";", result.Errors.Select(x => x.Description))}");
        }
    }

    private static string GetConnectionStringsByWizard()
    {
        var host = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 数据库主机地址 (不含端口)")
                .DefaultValue("localhost"));
        var port = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 数据库端口")
                .DefaultValue("3306"));
        var uid = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 数据库用户名")
                .DefaultValue("classisland_management"));
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 数据库密码")
                .Secret());
        var database = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 数据库名称")
                .DefaultValue("classisland_management"));
        
        return $"Host={host};Uid={uid};Password={password};Database={database};Port={port}";
    }

    private static string GetConnectionStringManual()
    {
        var cs = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan](?)[/] 请输入数据库连接字符串"));
        return cs;
    }
}