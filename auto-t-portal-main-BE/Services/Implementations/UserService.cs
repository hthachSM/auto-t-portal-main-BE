using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Models.DTOs.User;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class UserService(AppDbContext db) : IUserService
{
    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await db.Users
            .Include(u => u.Appointments)
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid id, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task<PagedResult<UserResponse>> GetAllAsync(UserFilterRequest filter)
    {
        var query = db.Users
            .Include(u => u.Appointments)
            .Include(u => u.Orders)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
        }

        if (!string.IsNullOrEmpty(filter.Role) &&
            Enum.TryParse<UserRole>(filter.Role, true, out var role))
        {
            query = query.Where(u => u.Role == role);
        }

        query = query.OrderByDescending(u => u.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<UserResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task UpdateRoleAsync(Guid id, UpdateUserRoleRequest request)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        user.Role = request.Role;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        user.IsDeleted = true;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    private static UserResponse MapToResponse(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Role = u.Role.ToString(),
        CreatedAt = u.CreatedAt,
        TotalAppointments = u.Appointments?.Count ?? 0,
        TotalOrders = u.Orders?.Count ?? 0
    };
}
