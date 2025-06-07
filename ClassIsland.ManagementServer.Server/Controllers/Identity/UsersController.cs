using System.Security.Claims;
using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.ManagementServer.Server.Services;
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
    ManagementServerContext dbContext,
    OrganizationSettingsService organizationSettingsService) : ControllerBase
{
    public ILogger<UsersController> Logger { get; } = logger;
    public UserManager<User> UserManager { get; } = userManager;
    public ManagementServerContext DbContext { get; } = dbContext;
    public OrganizationSettingsService OrganizationSettingsService { get; } = organizationSettingsService;

    [HttpPost("create")]
    [Authorize(Roles = Roles.UsersManager)]
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
    [Authorize(Roles = Roles.UsersManager)]
    public async Task<IActionResult> ListUsers([FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        var users = await UserManager.Users.Select(x => x)
            .ToPaginatedListAsync(pageIndex, pageSize);
        var result = users.Items
            .Select(x => GetUserInfoFromUser(x).Result)
            .ToList()
            .ToPaginatedList(pageIndex, pageSize);
        return Ok(result);
    }

    [HttpPost("{id:guid}")]
    [Authorize(Roles = Roles.UsersManager)]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfo body, [FromRoute] Guid id)
    {
        var info = await UserManager.FindByIdAsync(id.ToString());
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }

        var currentUser = await UserManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return BadRequest(new Error("登录状态无效"));
        }

        var currentRoles = await UserManager.GetRolesAsync(currentUser);

        var rolesRaw = await UserManager.GetRolesAsync(info);
        var rolesAdd = body.Roles.Where(x => !rolesRaw.Contains(x)).ToList();
        var rolesDelete = rolesRaw.Where(x => !body.Roles.Contains(x)).ToList();
        if (rolesAdd.Exists(x => !currentRoles.Contains(x)))
        {
            return BadRequest(new Error("不能为用户添加自身拥有的角色以外的角色"));
        }
        
        
        info.Name = body.Name;
        info.Email = body.EmailAddress;
        info.PhoneNumber = body.PhoneNumber;
        info.AllowChangePassword = body.AllowChangePassword;
        info.UpdatedTime = DateTime.Now;
        info.CreatedTime = info.CreatedTime;
        
        var result = await UserManager.UpdateAsync(info);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"更新用户信息失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }

        var resultAddRole = await UserManager.AddToRolesAsync(info, rolesAdd);
        if (!resultAddRole.Succeeded)
        {
            return BadRequest(new Error($"更新用户信息失败：{string.Join(';', resultAddRole.Errors.Select(e => e.Description))}"));
        }
        var resultDeleteRole = await UserManager.RemoveFromRolesAsync(info, rolesDelete);
        if (!resultDeleteRole.Succeeded)
        {
            return BadRequest(new Error($"更新用户信息失败：{string.Join(';', resultDeleteRole.Errors.Select(e => e.Description))}"));
        }
        
        return Ok(await GetUserInfoFromUser(info));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.UsersManager)]
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
    [Authorize(Roles = Roles.UsersManager)]
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

    private async Task<UserInfo> GetUserInfoFromUser(User user)
    {
        return new UserInfo()
        {
            UserName = user.UserName ?? "",
            Name = user.Name,
            EmailAddress = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Id = user.Id,
            AllowChangePassword = user.AllowChangePassword,
            CreatedTime = user.CreatedTime,
            UpdatedTime = user.UpdatedTime,
            Roles = (await UserManager.GetRolesAsync(user)).ToList(),
            RedirectToOobe = await OrganizationSettingsService.GetSettings("IsOobeCompleted") != "true"
                             && await UserManager.IsInRoleAsync(user, Roles.Admin)
        };
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var info = await UserManager.GetUserAsync(User);
        if (info == null)
        {
            return NotFound(new Error("当前用户信息未找到。"));
        }
        return Ok(await GetUserInfoFromUser(info));
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

        if (!info.AllowChangePassword)
        {
            return BadRequest(new Error("当前用户不允许修改密码"));
        }
        
        var result = await UserManager.ChangePasswordAsync(info, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new Error($"修改密码失败：{string.Join(';', result.Errors.Select(e => e.Description))}"));
        }
        
        return Ok();
    }
}