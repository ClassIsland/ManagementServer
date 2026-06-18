using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Protobuf.Command;
using ClassIsland.Shared.Protobuf.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/clients/{cuid}/automation")]
public class AutomationController(
    ILogger<AutomationController> logger,
    ManagementServerContext dbContext,
    ClientCommandDeliverService commandDeliverService,
    PendingConfigRequestService pendingConfigRequests) : ControllerBase
{
    public ILogger<AutomationController> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;
    public ClientCommandDeliverService CommandDeliverService { get; } = commandDeliverService;
    public PendingConfigRequestService PendingConfigRequests { get; } = pendingConfigRequests;

    /// <summary>
    /// 获取客户端自动化配置
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConfig(Guid cuid)
    {
        var config = await DbContext.Set<ClientAutomationConfig>().FindAsync(cuid);
        if (config == null)
            return Ok(new ClientAutomationConfig { ClientCuid = cuid, WorkflowsJson = "[]" });
        return Ok(config);
    }

    /// <summary>
    /// 更新客户端自动化配置并推送到客户端
    /// </summary>
    [HttpPut]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> UpdateConfig(Guid cuid, [FromBody] UpdateAutomationRequest request)
    {
        var config = await DbContext.Set<ClientAutomationConfig>().FindAsync(cuid);
        if (config == null)
        {
            config = new ClientAutomationConfig
            {
                ClientCuid = cuid,
                WorkflowsJson = request.WorkflowsJson ?? "[]",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };
            await DbContext.Set<ClientAutomationConfig>().AddAsync(config);
        }
        else
        {
            config.WorkflowsJson = request.WorkflowsJson ?? config.WorkflowsJson;
            config.UpdatedTime = DateTime.Now;
        }

        await DbContext.SaveChangesAsync();

        // 推送到客户端
        var pushPayload = new PushConfig
        {
            ConfigType = 4, // CurrentAutomation
            ConfigJson = config.WorkflowsJson
        };
        await CommandDeliverService.DeliverCommandAsync(
            CommandTypes.PushConfig,
            pushPayload,
            new ObjectsAssignee
            {
                AssigneeType = AssigneeTypes.ClientUid,
                TargetClientCuid = cuid
            });

        Logger.LogInformation("已更新客户端 {Cuid} 的自动化配置", cuid);
        return Ok(config);
    }

    /// <summary>
    /// 请求客户端上报当前自动化配置
    /// </summary>
    [HttpPost("request-upload")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> RequestUpload(Guid cuid)
    {
        var payload = new GetClientConfig
        {
            RequestGuid = Guid.NewGuid().ToString(),
            ConfigType = ConfigTypes.CurrentAutomation
        };

        PendingConfigRequests.TrackRequest(payload.RequestGuid, ConfigTypes.CurrentAutomation, cuid);

        await CommandDeliverService.DeliverCommandAsync(
            CommandTypes.GetClientConfig,
            payload,
            new ObjectsAssignee
            {
                AssigneeType = AssigneeTypes.ClientUid,
                TargetClientCuid = cuid
            });

        Logger.LogInformation("已请求客户端 {Cuid} 上报自动化配置", cuid);
        return Ok(new { success = true, requestId = payload.RequestGuid });
    }
}

public class UpdateAutomationRequest
{
    public string? WorkflowsJson { get; set; }
}
