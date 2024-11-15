namespace ClassIsland.ManagementServer.Server.Helpers;

public static class GuidHelpers
{
    public static Guid TryParseGuidOrEmpty(string? s)
    {
        return Guid.TryParse(s, out var guid) ? guid : Guid.Empty;
    }
}