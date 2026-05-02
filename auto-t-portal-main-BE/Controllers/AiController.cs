using auto_t_portal_main_BE.Models.DTOs.AI;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController(IAiService aiService) : ControllerBase
{
    // Public: Tìm xe bằng ngôn ngữ tự nhiên
    [HttpPost("search")]
    public async Task<IActionResult> SemanticSearch([FromBody] SemanticSearchRequest request)
    {
        var result = await aiService.SemanticSearchAsync(request);
        return Ok(result);
    }

    // Public: Định giá xe cũ
    [HttpPost("trade-in")]
    public async Task<IActionResult> TradeIn([FromBody] TradeInRequest request)
    {
        var result = await aiService.EstimateTradeInAsync(request);
        return Ok(result);
    }

    // Admin: Generate embeddings cho toàn bộ xe
    [HttpPost("embeddings/generate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GenerateEmbeddings()
    {
        await aiService.GenerateEmbeddingsForAllCarsAsync();
        return Ok(new { message = "Đã generate embeddings thành công." });
    }
}
