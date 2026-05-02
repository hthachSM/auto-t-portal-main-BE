namespace auto_t_portal_main_BE.Models.DTOs.Report;

public class CarStatusDistributionResponse
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
