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
[Route("api/v1/component-templates")]
public class ComponentTemplatesController(
    ILogger<ComponentTemplatesController> logger,
    ManagementServerContext dbContext) : ControllerBase
{
    public ILogger<ComponentTemplatesController> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var templates = await DbContext.ComponentTemplates.OrderByDescending(x => x.CreatedTime).ToListAsync();
        return Ok(templates);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var template = await DbContext.ComponentTemplates.FindAsync(id);
        if (template == null) return NotFound(new Error("模板不存在"));
        return Ok(template);
    }

    [HttpPost]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Create([FromBody] ComponentTemplate template)
    {
        template.Id = Guid.NewGuid();
        template.CreatedTime = DateTime.Now;
        template.UpdatedTime = DateTime.Now;
        await DbContext.ComponentTemplates.AddAsync(template);
        await DbContext.SaveChangesAsync();
        return Ok(template);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ComponentTemplate template)
    {
        var existing = await DbContext.ComponentTemplates.FindAsync(id);
        if (existing == null) return NotFound(new Error("模板不存在"));

        existing.Name = template.Name;
        existing.LayoutJson = template.LayoutJson;
        existing.PluginComponentTypes = template.PluginComponentTypes;
        existing.UpdatedTime = DateTime.Now;
        await DbContext.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var template = await DbContext.ComponentTemplates.FindAsync(id);
        if (template == null) return NotFound(new Error("模板不存在"));
        DbContext.ComponentTemplates.Remove(template);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTemplateRequest request)
    {
        var template = await DbContext.ComponentTemplates.FindAsync(id);
        if (template == null) return NotFound(new Error("模板不存在"));

        // 分配给指定客户端
        if (request.ClientCuids != null)
        {
            foreach (var cuid in request.ClientCuids)
            {
                var config = await DbContext.ClientComponentConfigs.FindAsync(cuid);
                if (config == null)
                {
                    config = new ClientComponentConfig
                    {
                        ClientCuid = cuid,
                        TemplateId = id,
                        LayoutJson = template.LayoutJson,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now
                    };
                    await DbContext.ClientComponentConfigs.AddAsync(config);
                }
                else
                {
                    config.TemplateId = id;
                    config.LayoutJson = template.LayoutJson;
                    config.UpdatedTime = DateTime.Now;
                }
            }
        }

        await DbContext.SaveChangesAsync();
        return Ok(new { success = true });
    }
}

[ApiController]
[Authorize]
[Route("api/v1/clients/{cuid}/components")]
public class ClientComponentsController(
    ILogger<ClientComponentsController> logger,
    ManagementServerContext dbContext,
    ClientCommandDeliverService commandDeliverService,
    PendingConfigRequestService pendingConfigRequests) : ControllerBase
{
    public ILogger<ClientComponentsController> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;
    public ClientCommandDeliverService CommandDeliverService { get; } = commandDeliverService;
    public PendingConfigRequestService PendingConfigRequests { get; } = pendingConfigRequests;

    /// <summary>
    /// 获取客户端组件配置
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConfig(Guid cuid)
    {
        var config = await DbContext.ClientComponentConfigs.FindAsync(cuid);
        if (config == null)
        {
            // 返回空配置
            return Ok(new ClientComponentConfig { ClientCuid = cuid, LayoutJson = "[]" });
        }
        return Ok(config);
    }

    /// <summary>
    /// 更新客户端组件配置并推送到客户端
    /// </summary>
    [HttpPut]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> UpdateConfig(Guid cuid, [FromBody] UpdateComponentConfigRequest request)
    {
        var config = await DbContext.ClientComponentConfigs.FindAsync(cuid);
        if (config == null)
        {
            config = new ClientComponentConfig
            {
                ClientCuid = cuid,
                LayoutJson = request.LayoutJson,
                OverrideJson = request.OverrideJson,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };
            await DbContext.ClientComponentConfigs.AddAsync(config);
        }
        else
        {
            config.LayoutJson = request.LayoutJson ?? config.LayoutJson;
            config.OverrideJson = request.OverrideJson;
            config.UpdatedTime = DateTime.Now;
        }

        await DbContext.SaveChangesAsync();

        // 推送到客户端
        var pushPayload = new PushConfig
        {
            ConfigType = 3, // CurrentComponent
            ConfigJson = config.LayoutJson
        };
        await CommandDeliverService.DeliverCommandAsync(
            CommandTypes.PushConfig,
            pushPayload,
            new ObjectsAssignee
            {
                AssigneeType = AssigneeTypes.ClientUid,
                TargetClientCuid = cuid
            });

        Logger.LogInformation("已推送组件配置到客户端 {Cuid}", cuid);
        return Ok(config);
    }

    /// <summary>
    /// 请求客户端上报当前组件配置
    /// </summary>
    [HttpPost("request-upload")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> RequestUpload(Guid cuid)
    {
        var payload = new GetClientConfig
        {
            RequestGuid = Guid.NewGuid().ToString(),
            ConfigType = ConfigTypes.CurrentComponent
        };

        PendingConfigRequests.TrackRequest(payload.RequestGuid, ConfigTypes.CurrentComponent, cuid);

        await CommandDeliverService.DeliverCommandAsync(
            CommandTypes.GetClientConfig,
            payload,
            new ObjectsAssignee
            {
                AssigneeType = AssigneeTypes.ClientUid,
                TargetClientCuid = cuid
            });

        Logger.LogInformation("已请求客户端 {Cuid} 上报组件配置", cuid);
        return Ok(new { success = true, requestId = payload.RequestGuid });
    }
}

public class AssignTemplateRequest
{
    public List<Guid>? ClientCuids { get; set; }
}

public class UpdateComponentConfigRequest
{
    public string? LayoutJson { get; set; }
    public string? OverrideJson { get; set; }
}
