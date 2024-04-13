using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientRegistryController(ManagementServerContext context) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = context;
    
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
            return BadRequest("Client already registered");
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
}