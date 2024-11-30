using System.Text;
using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Models.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/clients_registry")]
public class ClientRegistryController(ManagementServerContext context) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = context;

    /// <summary>
    /// 列举已经注册的实例。
    /// </summary>
    /// <returns>
    /// 返回已经注册的实例列表
    /// </returns>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await PaginatedList<Client>.CreateAsync(DataContext.Clients.Select(x => x), pageIndex, pageSize));
    }
    
    [HttpPost("register")]
    [Obsolete]
    public IActionResult Register([FromQuery] Guid cuid, [FromQuery] string id)
    {
        if (DataContext.Clients.Any(x => x.Cuid == cuid))
        {
            return BadRequest("这个实例已经注册。");
        }

        var newClient = new Client()
        {
            Cuid = cuid,
            Id = id,
            RegisterTime = DateTime.Now
        };
        DataContext.Clients.Add(newClient);
        DataContext.SaveChanges();
        return Ok(newClient);
    }
    
    [HttpDelete("unregister")]
    public IActionResult Unregister([FromQuery] Client client)
    {
        if (!DataContext.Clients.Any(x => x.Cuid == client.Cuid))
        {
            return BadRequest("实例还没有注册。");
        }
        DataContext.Clients.Remove(DataContext.Clients.First(x => x.Cuid == client.Cuid));
        DataContext.SaveChanges();
        return Ok();
    }
    
    [HttpGet("query/{cuid}")]
    public IActionResult Query([FromRoute]Guid cuid)
    {
        var c = DataContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (c == null)
        {
            return BadRequest("实例还没有注册。");
        }
        return Ok(c);
    }

    [AllowAnonymous]
    [HttpGet("client_configure")]
    public IActionResult DownloadClientConfigure()
    {
        // TODO: 返回真实服务器地址
        var json = JsonSerializer.Serialize(new ManagementSettings()
        {
            ManagementServer = "http://localhost:5090",
            ManagementServerGrpc = "http://localhost:5091",
            ManagementServerKind = ManagementServerKind.ManagementServer
        });
        var bytes = Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", "ManagementPreset.json");
    }
}