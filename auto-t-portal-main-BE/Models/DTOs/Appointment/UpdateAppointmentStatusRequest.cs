using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Appointment;

public class UpdateAppointmentStatusRequest
{
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
}
