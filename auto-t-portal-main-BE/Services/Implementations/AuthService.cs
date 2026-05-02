using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Helpers;
using auto_t_portal_main_BE.Models.DTOs.Auth;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class AuthService(AppDbContext db, JwtHelper jwt, IConfiguration config) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == request.Email.ToLower());
        if (exists)
            throw new InvalidOperationException("Email đã được sử dụng.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Customer
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == refreshToken &&
            u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn.");

        return await GenerateAuthResponse(user);
    }

    public async Task RevokeTokenAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId);
        if (user == null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = jwt.GenerateAccessToken(user);
        var refreshToken = jwt.GenerateRefreshToken();
        var expiryDays = int.Parse(config["Jwt:RefreshExpiryDays"]!);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(expiryDays);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(config["Jwt:ExpiryMinutes"]!)),
            User = new UserInfo
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };
    }
}
