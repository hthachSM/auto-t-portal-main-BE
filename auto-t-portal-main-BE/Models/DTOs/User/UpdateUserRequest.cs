using System.ComponentModel.DataAnnotations;
using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.User;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
}
