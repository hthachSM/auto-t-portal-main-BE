namespace auto_t_portal_main_BE.Models.DTOs.Report;

public class TopCarResponse
{
    public Guid CarId { get; set; }
    public string CarTitle { get; set; } = default!;
    public string? PrimaryImage { get; set; }
    public decimal Price { get; set; }
    public int AppointmentCount { get; set; }
    public int OrderCount { get; set; }
}
