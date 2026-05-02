using System.Security.Claims;
using auto_t_portal_main_BE.Models.DTOs.User;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Customer: Xem profile của mình
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await userService.GetByIdAsync(CurrentUserId);
        return Ok(result);
    }

    // Customer: Cập nhật profile
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        var result = await userService.UpdateProfileAsync(CurrentUserId, request);
        return Ok(result);
    }

    // Customer: Đổi mật khẩu
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await userService.ChangePasswordAsync(CurrentUserId, request);
        return Ok(new { message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại." });
    }

    // Admin: Lấy danh sách tất cả user
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] UserFilterRequest filter)
    {
        var result = await userService.GetAllAsync(filter);
        return Ok(result);
    }

    // Admin: Xem chi tiết user bất kỳ
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await userService.GetByIdAsync(id);
        return Ok(result);
    }

    // Admin: Cập nhật role user
    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        await userService.UpdateRoleAsync(id, request);
        return Ok(new { message = "Đã cập nhật quyền người dùng." });
    }

    // Admin: Vô hiệu hóa tài khoản
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await userService.DeactivateAsync(id);
        return Ok(new { message = "Đã vô hiệu hóa tài khoản." });
    }
}
