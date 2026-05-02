using auto_t_portal_main_BE.Models.DTOs.Payment;
using auto_t_portal_main_BE.Models.DTOs.Car;
using Microsoft.AspNetCore.Http;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentUrlResponse> CreatePaymentUrlAsync(Guid userId, CreatePaymentRequest request, string ipAddress);
    Task<bool> ProcessCallbackAsync(IQueryCollection query);
    Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize);
    Task<PagedResult<OrderResponse>> GetAllOrdersAsync(int page, int pageSize);
    Task<OrderResponse> GetOrderByIdAsync(Guid orderId);
}
