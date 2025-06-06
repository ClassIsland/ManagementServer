using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class OrganizationSettingsService(ManagementServerContext dbContext)
{
    public ManagementServerContext DbContext { get; } = dbContext;

    public async Task<string?> GetSettings(string key)
    {
        var entity = await DbContext.OrganizationSettings.FindAsync(key);
        return entity?.Value;
    }
    
    public async Task<Dictionary<string, string?>> GetSettingsByCategory(string category)
    {
        return await DbContext.OrganizationSettings
            .Where(s => s.Category == category)
            .ToDictionaryAsync(s => s.Key, s => s.Value);
    }
    
    public async Task SetOrganizationSettings(string key, string category, string? value)
    {
        var setting = await DbContext.OrganizationSettings.FindAsync(key);
        if (setting == null)
        {
            setting = new OrganizationSettings()
            {
                Key = key,
                Category = category,
                Value = value
            };
            DbContext.OrganizationSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            setting.Category = category;
            DbContext.OrganizationSettings.Update(setting);
        }
    }
}