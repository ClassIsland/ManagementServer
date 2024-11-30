using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Users;

[Route("/api/v1/users/")]
public class UsersInfoController(UserManager<User> userManager, ILogger<UsersInfoController> logger) : ControllerBase
{
    private UserManager<User> UserManager { get; } = userManager;
    private ILogger Logger { get; } = logger;

    [HttpGet("info/{id}")]
    public async Task<IActionResult> GetUserInfo([FromRoute] string id)
    {
        Logger.LogTrace("Getting user by ID {}", id);
        var user = await UserManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            return Ok(new UserInfo()
            {
                EmailAddress = user.Email ?? "",
                Name = user.Name ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            });
        }
        return NotFound();
    }
}