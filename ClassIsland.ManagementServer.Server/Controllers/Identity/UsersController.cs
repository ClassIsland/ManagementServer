using System.Security.Claims;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers.Identity;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public class UsersController(ILogger<UsersController> logger, UserManager<User> userManager) : ControllerBase
{
    public ILogger<UsersController> Logger { get; } = logger;
    public UserManager<User> UserManager { get; } = userManager;
    
    [HttpPost("manage")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateBody body)
    {
        var result = await UserManager.CreateAsync(body.User, body.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"创建用户失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }

        return Ok(body.User);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var info = await UserManager.GetUserAsync(User);
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }
        return Ok(new UserInfo()
        {
            UserName = info.UserName ?? "",
            Name = info.Name,
            EmailAddress = info.Email ?? "",
            PhoneNumber = info.PhoneNumber ?? "",
        });
    }
    
    [HttpPost("current")]
    public async Task<IActionResult> SetCurrentUserInfo([FromBody] UserInfo body)
    {
        var info = await UserManager.GetUserAsync(User);
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }

        info.Name = body.Name;
        info.Email = body.EmailAddress;
        info.PhoneNumber = body.PhoneNumber;
        
        var result = await UserManager.UpdateAsync(info);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"更新用户信息失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }
        return Ok(new UserInfo()
        {
            Name = info.Name,
            EmailAddress = info.Email ?? "",
            PhoneNumber = info.PhoneNumber ?? "",
        });
    }
    
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestBody request)
    {
        var info = await UserManager.GetUserAsync(User);
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }
        
        var result = await UserManager.ChangePasswordAsync(info, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"修改密码失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }
        
        return Ok();
    }
}