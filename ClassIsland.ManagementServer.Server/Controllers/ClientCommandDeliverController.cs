using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Command;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Protobuf.Enum;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/client-commands/")]
public class ClientCommandDeliverController(ClientCommandDeliverService clientCommandDeliverService) : ControllerBase
{
    private ClientCommandDeliverService ClientCommandDeliverService { get; } = clientCommandDeliverService;
    
    [HttpPost("show-notification")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        foreach (var target in request.Targets)
        {
            await ClientCommandDeliverService.DeliverCommandAsync(
                CommandTypes.SendNotification,
                request.Payload,
                target);
        }

        return Ok();
    }
    
    [HttpPost("restart-app")]
    public async Task<IActionResult> RestartApp([FromBody] RestartAppRequest request)
    {
        foreach (var target in request.Targets)
        {
            await ClientCommandDeliverService.DeliverCommandAsync(
                CommandTypes.RestartApp,
                new Empty(),
                target);
        }

        return Ok();
    }
}