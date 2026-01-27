namespace ClassIsland.ManagementServer.Server.Abstractions.Plugin;

public static class PluginVersionHelper
{
    public static bool IsVersionCompatible(string clientVersion, string minVersion, string? maxVersion)
    {
        if (!Version.TryParse(clientVersion, out var parsedClientVersion))
        {
            return false;
        }

        if (!Version.TryParse(minVersion, out var parsedMinVersion))
        {
            return false;
        }

        if (parsedClientVersion < parsedMinVersion)
        {
            return false;
        }

        if (maxVersion != null && Version.TryParse(maxVersion, out var parsedMaxVersion))
        {
            if (parsedClientVersion > parsedMaxVersion)
            {
                return false;
            }
        }

        return true;
    }

    public static Version? TryParseVersion(string versionString)
    {
        return Version.TryParse(versionString, out var version) ? version : null;
    }
}
