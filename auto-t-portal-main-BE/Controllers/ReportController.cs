using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class ReportController(IReportService reportService) : ControllerBase
{
    // Tổng quan dashboard
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await reportService.GetDashboardSummaryAsync();
        return Ok(result);
    }

    // Doanh thu theo tháng
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] int? year)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;
        var result = await reportService.GetRevenueByMonthAsync(targetYear);
        return Ok(result);
    }

    // Top xe được quan tâm nhiều nhất
    [HttpGet("top-cars")]
    public async Task<IActionResult> GetTopCars([FromQuery] int topN = 10)
    {
        var result = await reportService.GetTopCarsAsync(topN);
        return Ok(result);
    }

    // Phân bố trạng thái xe
    [HttpGet("car-status")]
    public async Task<IActionResult> GetCarStatusDistribution()
    {
        var result = await reportService.GetCarStatusDistributionAsync();
        return Ok(result);
    }

    // Xu hướng đặt lịch theo ngày
    [HttpGet("appointment-trend")]
    public async Task<IActionResult> GetAppointmentTrend([FromQuery] int days = 30)
    {
        if (days < 7 || days > 90)
            return BadRequest(new { error = "Số ngày phải từ 7 đến 90." });

        var result = await reportService.GetAppointmentTrendAsync(days);
        return Ok(result);
    }
}
