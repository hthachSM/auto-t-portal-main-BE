using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Models.Entities;

public enum AppointmentStatus { Pending = 1, Confirmed = 2, Cancelled = 3, Completed = 4 }

public class Appointment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
    public User User { get; set; } = default!;
    public Car Car { get; set; } = default!;
}