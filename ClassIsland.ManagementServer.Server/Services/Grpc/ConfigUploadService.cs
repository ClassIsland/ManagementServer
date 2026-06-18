using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ConfigUploadService(
    ILogger<ConfigUploadService> logger,
    ManagementServerContext dbContext,
    PendingConfigRequestService pendingRequests) : ConfigUpload.ConfigUploadBase
{
    public ILogger<ConfigUploadService> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;
    public PendingConfigRequestService PendingRequests { get; } = pendingRequests;

    public override async Task<ConfigUploadScRsp> UploadConfig(ConfigUploadScReq request, ServerCallContext context)
    {
        var ret = new ConfigUploadScRsp();
        try
        {
            var cuidStr = context.RequestHeaders.GetValue("cuid");
            if (string.IsNullOrEmpty(cuidStr) || !Guid.TryParse(cuidStr, out var cuid))
            {
                ret.Retcode = Retcode.InvalidRequest;
                ret.Message = "Invalid cuid";
                return ret;
            }

            var client = await DbContext.Clients.FindAsync(cuid);
            if (client == null)
            {
                ret.Retcode = Retcode.ClientNotFound;
                ret.Message = "Client not found";
                return ret;
            }

            // 根据 RequestGuid 查找原始请求类型
            var pending = PendingRequests.ConsumeRequest(request.RequestGuidId ?? "");
            var configType = pending?.ConfigType ?? ConfigTypes.UnspecifiedConfig;

            // 存储原始快照
            var snapshot = new ClientConfigSnapshot
            {
                ClientCuid = cuid,
                RequestGuidId = request.RequestGuidId ?? "",
                Payload = request.Payload ?? ""
            };
            await DbContext.ClientConfigSnapshots.AddAsync(snapshot);

            // 根据配置类型路由到正确的表
            switch (configType)
            {
                case ConfigTypes.CurrentComponent:
                    await RouteComponentConfig(cuid, request.Payload);
                    break;
                case ConfigTypes.CurrentAutomation:
                    await RouteAutomationConfig(cuid, request.Payload);
                    break;
                case ConfigTypes.PluginList:
                    await RoutePluginList(cuid, request.Payload);
                    break;
            }

            await DbContext.SaveChangesAsync();

            Logger.LogInformation("收到客户端 {Cuid} 的配置上传，类型: {Type}，请求ID: {RequestId}",
                cuid, configType, request.RequestGuidId);
            ret.Retcode = Retcode.Success;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "处理配置上传时出错");
            ret.Retcode = Retcode.ServerInternalError;
            ret.Message = e.Message;
        }

        return ret;
    }

    private async Task RouteComponentConfig(Guid cuid, string payload)
    {
        var existing = await DbContext.ClientComponentConfigs
            .FirstOrDefaultAsync(x => x.ClientCuid == cuid);
        if (existing != null)
        {
            existing.LayoutJson = payload;
            existing.UpdatedTime = DateTime.Now;
        }
        else
        {
            await DbContext.ClientComponentConfigs.AddAsync(new ClientComponentConfig
            {
                ClientCuid = cuid,
                LayoutJson = payload
            });
        }
        Logger.LogInformation("组件配置已路由到 ClientComponentConfigs");
    }

    private async Task RouteAutomationConfig(Guid cuid, string payload)
    {
        var existing = await DbContext.ClientAutomationConfigs
            .FirstOrDefaultAsync(x => x.ClientCuid == cuid);
        if (existing != null)
        {
            existing.WorkflowsJson = payload;
            existing.UpdatedTime = DateTime.Now;
        }
        else
        {
            await DbContext.ClientAutomationConfigs.AddAsync(new ClientAutomationConfig
            {
                ClientCuid = cuid,
                WorkflowsJson = payload
            });
        }
        Logger.LogInformation("自动化配置已路由到 ClientAutomationConfigs");
    }

    private async Task RoutePluginList(Guid cuid, string payload)
    {
        // 解析插件列表并更新 ClientPluginInfos
        try
        {
            var pluginIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(payload) ?? new();
            // 清除旧的插件记录
            var existing = await DbContext.ClientPluginInfos
                .Where(x => x.ClientCuid == cuid)
                .ToListAsync();
            DbContext.ClientPluginInfos.RemoveRange(existing);
            // 添加新记录
            foreach (var pluginId in pluginIds)
            {
                await DbContext.ClientPluginInfos.AddAsync(new ClientPluginInfo
                {
                    ClientCuid = cuid,
                    PluginId = pluginId,
                    LoadStatus = 1 // Loaded
                });
            }
            Logger.LogInformation("插件列表已路由到 ClientPluginInfos: {Count} 个插件", pluginIds.Count);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析插件列表失败");
        }
    }
}
