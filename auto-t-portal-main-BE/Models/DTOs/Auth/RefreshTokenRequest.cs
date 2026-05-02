using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = default!;
}
