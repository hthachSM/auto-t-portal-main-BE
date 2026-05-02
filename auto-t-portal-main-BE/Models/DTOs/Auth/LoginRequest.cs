using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Auth;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
