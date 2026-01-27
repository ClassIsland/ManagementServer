namespace ClassIsland.ManagementServer.Server.Abstractions.Plugin;

public interface IServerPlugin
{
    string Identifier { get; }
    
    string Name { get; }
    
    string Description { get; }
    
    string Version { get; }
    
    string MinClientVersion { get; }
    
    string? MaxClientVersion { get; }
    
    Task OnLoadAsync();
    
    Task OnUnloadAsync();
    
    Task<PluginMessageResponse> HandleMessageAsync(Guid clientId, string messageType, byte[] payload);
    
    bool IsCompatible(string clientPluginVersion);
}

public class PluginMessageResponse
{
    public int RetCode { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
