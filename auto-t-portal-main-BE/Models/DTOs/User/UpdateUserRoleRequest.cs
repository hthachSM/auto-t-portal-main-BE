using System.ComponentModel.DataAnnotations;
using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.User;

public class UpdateUserRoleRequest
{
    [Required]
    public UserRole Role { get; set; }
}
