using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Helpers;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Chỉ seed nếu chưa có user nào
        if (db.Users.Any()) return;

        var admin = new User
        {
            FullName = "Super Admin",
            Email = "admin@autot.vn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            PhoneNumber = "0901234567"
        };

        var staff = new User
        {
            FullName = "Nhan Vien AutoT",
            Email = "staff@autot.vn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
            Role = UserRole.Staff,
            PhoneNumber = "0907654321"
        };

        db.Users.AddRange(admin, staff);
        await db.SaveChangesAsync();

        Console.WriteLine("✅ Seed data completed:");
        Console.WriteLine("   Admin: admin@autot.vn / Admin@123");
        Console.WriteLine("   Staff: staff@autot.vn / Staff@123");
    }
}
