using System.Text;
using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Models.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
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
    [HttpGet("all")]
    public async Task<IActionResult> List([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DataContext.Clients.Select(x => x).ToPaginatedListAsync(pageIndex, pageSize));
    }
    
    [HttpGet("abstract")]
    public async Task<IActionResult> ListAbstract([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DataContext.AbstractClients.Include(b => b.Group).Select(x => x)
            .ToPaginatedListAsync(pageIndex, pageSize));
    }
    
    [HttpPut("abstract")]
    public async Task<IActionResult> CreateAbstract([FromBody] AbstractClient group)
    {
        await DataContext.AbstractClients.AddAsync(group);
        await DataContext.SaveChangesAsync();
        
        return Ok(group);
    }
    
    [HttpPut("abstract/{id:long}")]
    public async Task<IActionResult> UpdateAbstract(long id, [FromBody] AbstractClient client)
    {   
        var prev = await DataContext.AbstractClients.AnyAsync(x => x.InternalId == id);
        if (!prev)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        DataContext.Entry(client).State = EntityState.Modified;
        await DataContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpDelete("abstract/{id:long}")]
    public async Task<IActionResult> DeleteAbstract(long id)
    {
        var client = await DataContext.AbstractClients.FirstOrDefaultAsync(x => x.InternalId == id);
        if (client == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DataContext.AbstractClients.Remove(client);
        await DataContext.SaveChangesAsync();
        return Ok();
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