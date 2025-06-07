using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ClassIsland.ManagementServer.Server.Authorization;

public class AdminAccessHandler : AuthorizationHandler<AdminAccessRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminAccessRequirement requirement)
    {
        if (context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement); // 放行
        }

        return Task.CompletedTask;
    }
}