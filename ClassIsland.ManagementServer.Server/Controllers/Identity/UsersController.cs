using System.Security.Claims;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Identity;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public class UsersController(ILogger<UsersController> logger, 
    UserManager<User> userManager,
    ManagementServerContext dbContext) : ControllerBase
{
    public ILogger<UsersController> Logger { get; } = logger;
    public UserManager<User> UserManager { get; } = userManager;
    public ManagementServerContext DbContext { get; } = dbContext;

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateBody body)
    {
        var result = await UserManager.CreateAsync(body.User, body.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"创建用户失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }

        return Ok(body.User);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> ListUsers([FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        var users = await UserManager.Users.Select(x => x)
            .ToPaginatedListAsync(pageIndex, pageSize);
        var result = users.Items.Select(x => new UserInfo()
        {
            EmailAddress = x.Email ?? "",
            UserName = x.UserName ?? "",
            Name = x.Name,
            PhoneNumber = x.PhoneNumber ?? "",
            Id = x.Id
        }).ToList().ToPaginatedList(pageIndex, pageSize);
        return Ok(result);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfo body, [FromRoute] Guid id)
    {
        var info = await UserManager.FindByIdAsync(id.ToString());
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }

        info.Name = body.Name;
        info.Email = body.EmailAddress;
        info.PhoneNumber = body.PhoneNumber;
        info.UpdatedTime = DateTime.Now;
        info.CreatedTime = info.CreatedTime;
        
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var info = await UserManager.FindByIdAsync(id.ToString());
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }

        if (info.UserName == "root")
        {
            return BadRequest(new Error("无法删除 root 用户"));
        }
        var result = await UserManager.DeleteAsync(info);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"删除用户失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }

        return Ok();
    }

    [HttpPost("{id:guid}/set-password")]
    public async Task<IActionResult> SetUserPassword([FromBody] ChangePasswordRequestBody body, [FromRoute] Guid id)
    {
        var info = await UserManager.FindByIdAsync(id.ToString());
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }

        var resultRemovePassword = await UserManager.RemovePasswordAsync(info);
        if (!resultRemovePassword.Succeeded)
        {
            return BadRequest(new Error($"设置密码失败：{string.Join(';', resultRemovePassword.Errors.Select(e => e.Description))}"));
        }
        
        var resultAddPassword = await UserManager.AddPasswordAsync(info, body.NewPassword);
        if (!resultAddPassword.Succeeded)
        {
            return BadRequest(new Error($"设置密码失败：{string.Join(';', resultAddPassword.Errors.Select(e => e.Description))}"));
        }

        return Ok();
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
            Id = info.Id
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
        info.UpdatedTime = DateTime.Now;
        info.CreatedTime = info.CreatedTime;
        
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