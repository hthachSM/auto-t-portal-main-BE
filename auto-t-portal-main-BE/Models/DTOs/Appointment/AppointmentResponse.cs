namespace auto_t_portal_main_BE.Models.DTOs.Appointment;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public string? UserPhone { get; set; }
    public Guid CarId { get; set; }
    public string CarTitle { get; set; } = default!;  // Make + Model + Year
    public string? CarPrimaryImage { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
