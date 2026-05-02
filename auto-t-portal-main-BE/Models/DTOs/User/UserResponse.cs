namespace auto_t_portal_main_BE.Models.DTOs.User;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public int TotalAppointments { get; set; }
    public int TotalOrders { get; set; }
}
