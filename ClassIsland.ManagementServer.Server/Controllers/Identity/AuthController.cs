using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClassIsland.ManagementServer.Server.Controllers.Identity;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController(ILogger<AuthController> logger,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
    TimeProvider timeProvider) : ControllerBase
{
    public ILogger<AuthController> Logger { get; } = logger;
    public UserManager<User> UserManager { get; } = userManager;
    public SignInManager<User> SignInManager { get; } = signInManager;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestBody request)
    {
        var user = await UserManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return NotFound(new Error($"找不到用户 {request.Username}"));
        }
        
        var result = await SignInManager.PasswordSignInAsync(user, request.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new Error("密码错误。"));
        }

        var principal = await SignInManager.CreateUserPrincipalAsync(user);

        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        return SignIn(principal, IdentityConstants.BearerScheme);
        
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await SignInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
        {
            return Challenge();
        }

        var newPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        return SignIn(newPrincipal, IdentityConstants.BearerScheme);
    }
}