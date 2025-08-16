using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Models.CyreneMsp;
using Grpc.Core;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class CyreneMspConnectionService(ILogger<CyreneMspConnectionService> logger, IServiceProvider serviceProvider)
{
    private static readonly TimeSpan InActivateSessionExpirationTime = TimeSpan.FromSeconds(30); 
    
    public Dictionary<Guid, ClientSession> Sessions { get; } = new();
    
    public ILogger<CyreneMspConnectionService> Logger { get; } = logger;
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public string ServerPublicKeyArmored { get; private set; } = null!;
    
    public EncryptionKeys ServerKey { get; private set; } = null!;
    
    private bool _initialized = false;
    
    public async Task InitServerKey()
    {
        Logger.LogInformation("正在加载服务端密钥");
        using var scope = ServiceProvider.CreateScope();
        var organizationSettingsService = scope.ServiceProvider.GetRequiredService<OrganizationSettingsService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ManagementServerContext>();
        var serverPrivateKey = await organizationSettingsService.GetSettings("CyreneMspPrivateKey");
        if (serverPrivateKey == null)
        {
            Logger.LogInformation("服务端未生成密钥，正在生成密钥");
            
            using var pgp = new PGP();
            using var publicKeyStream = new MemoryStream();
            using var privateKeyStream = new MemoryStream();
            
            await pgp.GenerateKeyAsync(publicKeyStream, privateKeyStream);

            publicKeyStream.Position = 0;
            privateKeyStream.Position = 0;
            using var publicReader = new StreamReader(publicKeyStream);
            using var privateReader = new StreamReader(privateKeyStream);
            var publicKey = await publicReader.ReadToEndAsync();
            var privateKey = await privateReader.ReadToEndAsync();
            await organizationSettingsService.SetOrganizationSettings("CyreneMspPublicKey", "CyreneMsp", publicKey);
            await organizationSettingsService.SetOrganizationSettings("CyreneMspPrivateKey", "CyreneMsp", privateKey);
            await dbContext.SaveChangesAsync();
            Logger.LogInformation("成功生成密钥");
        }

        serverPrivateKey ??= await organizationSettingsService.GetSettings("CyreneMspPrivateKey");
        var serverPublicKey = ServerPublicKeyArmored =
            (await organizationSettingsService.GetSettings("CyreneMspPublicKey"))!;

        ServerKey = new EncryptionKeys(serverPublicKey, serverPrivateKey, "");
        Logger.LogInformation("已加载服务端密钥");
        _initialized = true;
    }

    public bool EstablishClientConnection(Guid clientUid, [NotNullWhen(true)] out string? sessionId)
    {
        sessionId = null;
        if (Sessions.TryGetValue(clientUid, out var prevSession) && 
            !prevSession.IsActivated && prevSession.InvalidateTime + InActivateSessionExpirationTime > DateTime.Now )
        {
            return false;
        }
        sessionId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var session = new ClientSession()
        {
            ClientUid = clientUid,
            SessionId = sessionId,
            InvalidateTime = DateTime.Now
        };
        Sessions[clientUid] = session;
        return true;
    }

    public void DestroyConnection(Guid clientUid)
    {
        if (!Sessions.TryGetValue(clientUid, out var session))
        {
            return;
        }

        session.IsActivated = false;
        Sessions.Remove(clientUid);
    }
}