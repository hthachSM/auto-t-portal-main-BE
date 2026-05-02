using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController(ICarService carService) : ControllerBase
{
    // Public: Xem danh sách xe
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CarFilterRequest filter)
    {
        var result = await carService.GetAllAsync(filter);
        return Ok(result);
    }

    // Public: Xem chi tiết xe
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await carService.GetByIdAsync(id);
        return Ok(result);
    }

    // Admin/Staff: Thêm xe
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] CreateCarRequest request)
    {
        var result = await carService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Admin/Staff: Sửa xe
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCarRequest request)
    {
        var result = await carService.UpdateAsync(id, request);
        return Ok(result);
    }

    // Admin: Xóa xe (soft delete)
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await carService.DeleteAsync(id);
        return Ok(new { message = "Đã xóa xe thành công." });
    }

    // Admin/Staff: Cập nhật trạng thái xe
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        await carService.UpdateStatusAsync(id, status);
        return Ok(new { message = "Đã cập nhật trạng thái." });
    }

    // Admin/Staff: Upload ảnh xe
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> AddImage(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "File không hợp lệ." });

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowed.Contains(ext))
            return BadRequest(new { error = "Chỉ chấp nhận file ảnh jpg, png, webp." });

        var result = await carService.AddImageAsync(id, file);
        return Ok(result);
    }

    // Admin/Staff: Xóa ảnh xe
    [HttpDelete("images/{imageId:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        await carService.DeleteImageAsync(imageId);
        return Ok(new { message = "Đã xóa ảnh." });
    }
}
