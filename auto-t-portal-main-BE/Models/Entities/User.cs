using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Models.Entities;

public enum UserRole { Customer = 1, Staff = 2, Admin = 3 }

public class User : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}