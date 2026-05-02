using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Appointment;

public class AppointmentFilterRequest
{
    public AppointmentStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CarId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
