namespace auto_t_portal_main_BE.Models.DTOs.Report;

public class AppointmentTrendResponse
{
    public string DateLabel { get; set; } = default!;
    public int Total { get; set; }
    public int Confirmed { get; set; }
    public int Cancelled { get; set; }
    public int Completed { get; set; }
}
