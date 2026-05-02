using System.Security.Claims;
using auto_t_portal_main_BE.Models.DTOs.Auth;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await authService.RevokeTokenAsync(userId);
        return Ok(new { message = "Đăng xuất thành công." });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new UserInfo
        {
            Id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            FullName = User.FindFirstValue(ClaimTypes.Name)!,
            Email = User.FindFirstValue(ClaimTypes.Email)!,
            Role = User.FindFirstValue(ClaimTypes.Role)!
        });
    }
}
