using System.Security.Claims;
using auto_t_portal_main_BE.Models.DTOs.Appointment;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // Customer: Đặt lịch lái thử
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var result = await appointmentService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Customer: Xem lịch hẹn của mình
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await appointmentService.GetMyAppointmentsAsync(
            CurrentUserId, page, pageSize);
        return Ok(result);
    }

    // Customer: Hủy lịch hẹn của mình
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await appointmentService.CancelAsync(id, CurrentUserId);
        return Ok(new { message = "Đã hủy lịch hẹn." });
    }

    // Admin/Staff: Xem tất cả lịch hẹn
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll([FromQuery] AppointmentFilterRequest filter)
    {
        var result = await appointmentService.GetAllAsync(filter);
        return Ok(result);
    }

    // Admin/Staff: Xem chi tiết lịch hẹn
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await appointmentService.GetByIdAsync(id);
        return Ok(result);
    }

    // Admin/Staff: Cập nhật trạng thái lịch hẹn
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateAppointmentStatusRequest request)
    {
        var result = await appointmentService.UpdateStatusAsync(id, request);
        return Ok(result);
    }
}
