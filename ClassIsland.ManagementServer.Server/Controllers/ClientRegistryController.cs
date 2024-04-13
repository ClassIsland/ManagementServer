using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/clients")]
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
    public IActionResult Register([FromQuery] Client client)
    {
        if (DataContext.Clients.Any(x => x.Cuid == client.Cuid))
        {
            return BadRequest("这个实例已经注册。");
        }
        DataContext.Clients.Add(new Client()
        {
            Cuid = client.Cuid,
            Id = client.Id,
            RegisterTime = DateTime.Now
        });
        DataContext.SaveChanges();
        return Ok();
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