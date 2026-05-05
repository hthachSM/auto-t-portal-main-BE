using auto_t_portal_main_BE.Models.DTOs.Contract;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractController(IContractService contractService) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Admin/Staff: Tạo hợp đồng
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] CreateContractRequest request)
    {
        var result = await contractService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Admin/Staff: Xem tất cả hợp đồng
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll()
    {
        var result = await contractService.GetAllAsync();
        return Ok(result);
    }

    // Xem chi tiết hợp đồng
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await contractService.GetByIdAsync(id);
        return Ok(result);
    }

    // Customer: Xem hợp đồng của mình
    [HttpGet("my")]
    public async Task<IActionResult> GetMyContracts()
    {
        var result = await contractService.GetByUserIdAsync(CurrentUserId);
        return Ok(result);
    }

    // Admin/Staff: Cập nhật trạng thái hợp đồng
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateContractStatusRequest request)
    {
        var result = await contractService.UpdateStatusAsync(id, request);
        return Ok(result);
    }
}
