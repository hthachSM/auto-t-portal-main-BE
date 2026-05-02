using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Appointment;
using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class AppointmentService(AppDbContext db) : IAppointmentService
{
    public async Task<AppointmentResponse> CreateAsync(Guid userId, CreateAppointmentRequest request)
    {
        // Kiểm tra xe tồn tại và còn available
        var car = await db.Cars.Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == request.CarId)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        if (car.Status == CarStatus.Sold)
            throw new InvalidOperationException("Xe này đã được bán.");

        // Kiểm tra slot thời gian (không cho đặt trùng giờ, cách nhau ít nhất 1 tiếng)
        var conflictExists = await db.Appointments
            .AnyAsync(a =>
                a.CarId == request.CarId &&
                a.Status != AppointmentStatus.Cancelled &&
                Math.Abs(EF.Functions.DateDiffMinute(a.ScheduledAt, request.ScheduledAt)) < 60);

        if (conflictExists)
            throw new InvalidOperationException("Khung giờ này đã có lịch hẹn. Vui lòng chọn giờ khác.");

        // Kiểm tra không cho đặt lịch trong quá khứ
        if (request.ScheduledAt < DateTime.UtcNow.AddHours(2))
            throw new InvalidOperationException("Lịch hẹn phải đặt trước ít nhất 2 giờ.");

        var user = await db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        var appointment = new Appointment
        {
            UserId = userId,
            CarId = request.CarId,
            ScheduledAt = request.ScheduledAt,
            Notes = request.Notes,
            Status = AppointmentStatus.Pending
        };

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();

        appointment.User = user;
        appointment.Car = car;
        return MapToResponse(appointment);
    }

    public async Task<AppointmentResponse> UpdateStatusAsync(Guid id, UpdateAppointmentStatusRequest request)
    {
        var appointment = await db.Appointments
            .Include(a => a.User)
            .Include(a => a.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

        appointment.Status = request.Status;
        if (request.Notes != null) appointment.Notes = request.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return MapToResponse(appointment);
    }

    public async Task CancelAsync(Guid id, Guid userId)
    {
        var appointment = await db.Appointments
            .Include(a => a.User)
            .Include(a => a.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

        // Chỉ cho phép hủy nếu là chủ lịch hẹn hoặc staff/admin
        if (appointment.UserId != userId)
            throw new UnauthorizedAccessException("Bạn không có quyền hủy lịch hẹn này.");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Không thể hủy lịch hẹn đã hoàn thành.");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<AppointmentResponse> GetByIdAsync(Guid id)
    {
        var appointment = await db.Appointments
            .Include(a => a.User)
            .Include(a => a.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

        return MapToResponse(appointment);
    }

    public async Task<PagedResult<AppointmentResponse>> GetAllAsync(AppointmentFilterRequest filter)
    {
        var query = db.Appointments
            .Include(a => a.User)
            .Include(a => a.Car).ThenInclude(c => c.Images)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);
        if (filter.FromDate.HasValue)
            query = query.Where(a => a.ScheduledAt >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            query = query.Where(a => a.ScheduledAt <= filter.ToDate.Value);
        if (filter.UserId.HasValue)
            query = query.Where(a => a.UserId == filter.UserId.Value);
        if (filter.CarId.HasValue)
            query = query.Where(a => a.CarId == filter.CarId.Value);

        query = query.OrderByDescending(a => a.ScheduledAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<AppointmentResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<PagedResult<AppointmentResponse>> GetMyAppointmentsAsync(
        Guid userId, int page, int pageSize)
    {
        var query = db.Appointments
            .Include(a => a.User)
            .Include(a => a.Car).ThenInclude(c => c.Images)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ScheduledAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<AppointmentResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private static AppointmentResponse MapToResponse(Appointment a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        UserFullName = a.User.FullName,
        UserEmail = a.User.Email,
        UserPhone = a.User.PhoneNumber,
        CarId = a.CarId,
        CarTitle = $"{a.Car.Make} {a.Car.Model} {a.Car.Year}",
        CarPrimaryImage = a.Car.Images.FirstOrDefault(i => i.IsPrimary)?.Url
            ?? a.Car.Images.FirstOrDefault()?.Url,
        ScheduledAt = a.ScheduledAt,
        Status = a.Status.ToString(),
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };
}
