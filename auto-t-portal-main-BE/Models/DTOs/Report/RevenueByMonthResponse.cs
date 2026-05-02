namespace auto_t_portal_main_BE.Models.DTOs.Report;

public class RevenueByMonthResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthLabel { get; set; } = default!;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
