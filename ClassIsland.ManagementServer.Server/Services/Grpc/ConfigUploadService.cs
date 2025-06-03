using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ConfigUploadService : ConfigUpload.ConfigUploadBase
{
    public override async Task<ConfigUploadScRsp> UploadConfig(ConfigUploadScReq request, ServerCallContext context)
    {
        // todo
        return new ConfigUploadScRsp();
    }
}