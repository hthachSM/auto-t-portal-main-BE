using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Appointment;

public class CreateAppointmentRequest
{
    [Required]
    public Guid CarId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    public string? Notes { get; set; }
}
