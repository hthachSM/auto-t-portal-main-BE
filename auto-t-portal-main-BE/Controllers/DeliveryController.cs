using auto_t_portal_main_BE.Models.DTOs.Delivery;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class DeliveryController(IDeliveryService deliveryService) : ControllerBase
{
    // Tạo lịch bàn giao xe
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryRequest request)
    {
        var result = await deliveryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Xem tất cả lịch bàn giao
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await deliveryService.GetAllAsync();
        return Ok(result);
    }

    // Xem chi tiết lịch bàn giao
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await deliveryService.GetByIdAsync(id);
        return Ok(result);
    }

    // Xem lịch bàn giao theo hợp đồng
    [HttpGet("contract/{contractId:guid}")]
    public async Task<IActionResult> GetByContractId(Guid contractId)
    {
        var result = await deliveryService.GetByContractIdAsync(contractId);
        return Ok(result);
    }

    // Hoàn thành bàn giao xe (checklist)
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid id, [FromBody] CompleteDeliveryRequest request)
    {
        var result = await deliveryService.CompleteAsync(id, request);
        return Ok(result);
    }
}
