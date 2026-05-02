using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Models.DTOs.User;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<UserResponse> UpdateProfileAsync(Guid id, UpdateUserRequest request);
    Task ChangePasswordAsync(Guid id, ChangePasswordRequest request);
    Task<PagedResult<UserResponse>> GetAllAsync(UserFilterRequest filter);
    Task UpdateRoleAsync(Guid id, UpdateUserRoleRequest request);
    Task DeactivateAsync(Guid id);
}
