using auto_t_portal_main_BE.Models.DTOs.Car;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface ICarService
{
    Task<CarResponse> CreateAsync(CreateCarRequest request);
    Task<CarResponse> UpdateAsync(Guid id, UpdateCarRequest request);
    Task DeleteAsync(Guid id);
    Task<CarResponse> GetByIdAsync(Guid id);
    Task<PagedResult<CarResponse>> GetAllAsync(CarFilterRequest filter);
    Task<CarResponse> AddImageAsync(Guid carId, IFormFile file);
    Task DeleteImageAsync(Guid imageId);
    Task UpdateStatusAsync(Guid id, string status);
}
