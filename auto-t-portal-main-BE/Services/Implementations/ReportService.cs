using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Report;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class ReportService(AppDbContext db) : IReportService
{
    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var carStats = await db.Cars
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalRevenue = await db.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.Success)
            .SumAsync(o => o.DepositAmount);

        var revenueThisMonth = await db.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.Success &&
                        o.PaidAt >= startOfMonth)
            .SumAsync(o => o.DepositAmount);

        return new DashboardSummaryResponse
        {
            TotalCars = carStats.Sum(c => c.Count),
            AvailableCars = carStats.FirstOrDefault(c => c.Status == CarStatus.Available)?.Count ?? 0,
            ReservedCars = carStats.FirstOrDefault(c => c.Status == CarStatus.Reserved)?.Count ?? 0,
            SoldCars = carStats.FirstOrDefault(c => c.Status == CarStatus.Sold)?.Count ?? 0,
            TotalUsers = await db.Users.CountAsync(),
            NewUsersThisMonth = await db.Users.CountAsync(u => u.CreatedAt >= startOfMonth),
            TotalAppointments = await db.Appointments.CountAsync(),
            PendingAppointments = await db.Appointments
                .CountAsync(a => a.Status == AppointmentStatus.Pending),
            TotalRevenue = totalRevenue,
            RevenueThisMonth = revenueThisMonth,
            TotalOrders = await db.Orders.CountAsync(),
            SuccessfulOrders = await db.Orders
                .CountAsync(o => o.PaymentStatus == PaymentStatus.Success)
        };
    }

    public async Task<List<RevenueByMonthResponse>> GetRevenueByMonthAsync(int year)
    {
        var data = await db.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.Success &&
                        o.PaidAt.HasValue &&
                        o.PaidAt.Value.Year == year)
            .GroupBy(o => o.PaidAt!.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                Revenue = g.Sum(o => o.DepositAmount),
                OrderCount = g.Count()
            })
            .OrderBy(g => g.Month)
            .ToListAsync();

        var monthNames = new[]
        {
            "", "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
            "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
            "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
        };

        // Đảm bảo đủ 12 tháng, tháng không có doanh thu = 0
        return Enumerable.Range(1, 12).Select(month =>
        {
            var found = data.FirstOrDefault(d => d.Month == month);
            return new RevenueByMonthResponse
            {
                Year = year,
                Month = month,
                MonthLabel = monthNames[month],
                Revenue = found?.Revenue ?? 0,
                OrderCount = found?.OrderCount ?? 0
            };
        }).ToList();
    }

    public async Task<List<TopCarResponse>> GetTopCarsAsync(int topN = 10)
    {
        var topCars = await db.Cars
            .Include(c => c.Images)
            .Include(c => c.Appointments)
            .Include(c => c.Orders)
            .Select(c => new TopCarResponse
            {
                CarId = c.Id,
                CarTitle = $"{c.Make} {c.Model} {c.Year}",
                PrimaryImage = c.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? c.Images.FirstOrDefault(i => i.IsPrimary)!.Url
                    : c.Images.FirstOrDefault() != null
                        ? c.Images.FirstOrDefault()!.Url
                        : null,
                Price = c.Price,
                AppointmentCount = c.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled),
                OrderCount = c.Orders.Count(o => o.PaymentStatus == PaymentStatus.Success)
            })
            .OrderByDescending(c => c.AppointmentCount + c.OrderCount * 3)
            .Take(topN)
            .ToListAsync();

        return topCars;
    }

    public async Task<List<CarStatusDistributionResponse>> GetCarStatusDistributionAsync()
    {
        var total = await db.Cars.CountAsync();
        if (total == 0) return [];

        var data = await db.Cars
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return data.Select(d => new CarStatusDistributionResponse
        {
            Status = d.Status.ToString(),
            Count = d.Count,
            Percentage = Math.Round((double)d.Count / total * 100, 1)
        }).ToList();
    }

    public async Task<List<AppointmentTrendResponse>> GetAppointmentTrendAsync(int days = 30)
    {
        var fromDate = DateTime.UtcNow.Date.AddDays(-days + 1);

        var data = await db.Appointments
            .Where(a => a.CreatedAt >= fromDate)
            .GroupBy(a => a.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Confirmed = g.Count(a => a.Status == AppointmentStatus.Confirmed),
                Cancelled = g.Count(a => a.Status == AppointmentStatus.Cancelled),
                Completed = g.Count(a => a.Status == AppointmentStatus.Completed)
            })
            .OrderBy(g => g.Date)
            .ToListAsync();

        // Đảm bảo đủ số ngày, ngày không có lịch = 0
        return Enumerable.Range(0, days).Select(i =>
        {
            var date = fromDate.AddDays(i);
            var found = data.FirstOrDefault(d => d.Date == date);
            return new AppointmentTrendResponse
            {
                DateLabel = date.ToString("dd/MM"),
                Total = found?.Total ?? 0,
                Confirmed = found?.Confirmed ?? 0,
                Cancelled = found?.Cancelled ?? 0,
                Completed = found?.Completed ?? 0
            };
        }).ToList();
    }
}
