using auto_t_portal_main_BE.Models.DTOs.Car;

namespace auto_t_portal_main_BE.Models.DTOs.User;

public class UserFilterRequest
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
