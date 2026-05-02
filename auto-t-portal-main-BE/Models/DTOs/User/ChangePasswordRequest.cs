using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.User;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = default!;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = default!;
}
