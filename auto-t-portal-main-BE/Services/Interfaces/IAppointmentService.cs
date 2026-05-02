using auto_t_portal_main_BE.Models.DTOs.Appointment;
using auto_t_portal_main_BE.Models.DTOs.Car;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponse> CreateAsync(Guid userId, CreateAppointmentRequest request);
    Task<AppointmentResponse> UpdateStatusAsync(Guid id, UpdateAppointmentStatusRequest request);
    Task CancelAsync(Guid id, Guid userId);
    Task<AppointmentResponse> GetByIdAsync(Guid id);
    Task<PagedResult<AppointmentResponse>> GetAllAsync(AppointmentFilterRequest filter);
    Task<PagedResult<AppointmentResponse>> GetMyAppointmentsAsync(Guid userId, int page, int pageSize);
}
