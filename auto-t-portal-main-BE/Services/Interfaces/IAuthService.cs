using auto_t_portal_main_BE.Models.DTOs.Auth;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(Guid userId);
}
