using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/audit")]
public class AuditController(
    ILogger<AuditController> logger,
    ManagementServerContext dbContext) : ControllerBase
{
    public ILogger<AuditController> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> ListLogs([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await DbContext.AuditLogs
            .ToPaginatedListAsync(pageIndex, pageSize, decreasing: true, orderByUpdatedTime: true);
        return Ok(result);
    }
}
