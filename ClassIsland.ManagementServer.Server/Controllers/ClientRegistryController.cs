using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
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
    public IActionResult List()
    {
        return Ok(DataContext.Clients.ToList());
    }
    
    [HttpPost("register")]
    public IActionResult Register([FromQuery] string cuid, [FromQuery] string id)
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
    public IActionResult Query([FromRoute]string cuid)
    {
        var c = DataContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (c == null)
        {
            return BadRequest("实例还没有注册。");
        }
        return Ok(c);
    }
}