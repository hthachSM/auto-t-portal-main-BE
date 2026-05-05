using auto_t_portal_main_BE.Models.DTOs.Delivery;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IDeliveryService
{
    Task<DeliveryResponse> CreateAsync(CreateDeliveryRequest request);
    Task<DeliveryResponse> CompleteAsync(Guid id, CompleteDeliveryRequest request);
    Task<DeliveryResponse> GetByIdAsync(Guid id);
    Task<DeliveryResponse> GetByContractIdAsync(Guid contractId);
    Task<List<DeliveryResponse>> GetAllAsync();
}
