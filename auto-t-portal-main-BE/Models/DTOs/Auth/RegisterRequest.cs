using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required, MinLength(6)]
    public string Password { get; set; } = default!;

    public string? PhoneNumber { get; set; }
}