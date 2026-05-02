using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Helpers;
using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Models.DTOs.Payment;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class PaymentService(AppDbContext db, VnPayHelper vnPay, IConfiguration config) : IPaymentService
{
    public async Task<PaymentUrlResponse> CreatePaymentUrlAsync(
        Guid userId, CreatePaymentRequest request, string ipAddress)
    {
        var car = await db.Cars.FindAsync(request.CarId)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        if (car.Status != CarStatus.Available)
            throw new InvalidOperationException("Xe này hiện không thể đặt cọc.");

        // Tạo Order ở trạng thái Pending
        var order = new Order
        {
            UserId = userId,
            CarId = request.CarId,
            DepositAmount = request.DepositAmount,
            PaymentStatus = PaymentStatus.Pending
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        // Build VNPay URL
        var txnRef = order.Id.ToString("N"); // Không có dấu gạch ngang
        var orderInfo = request.OrderInfo ?? $"Dat coc xe {car.Make} {car.Model} {car.Year}";
        var returnUrl = config["VnPay:ReturnUrl"]!;
        var tmnCode = config["VnPay:TmnCode"]!;
        var createDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
        var expireDate = DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss");

        vnPay.AddRequestData("vnp_Version", "2.1.0");
        vnPay.AddRequestData("vnp_Command", "pay");
        vnPay.AddRequestData("vnp_TmnCode", tmnCode);
        vnPay.AddRequestData("vnp_Amount", ((long)(request.DepositAmount * 100)).ToString());
        vnPay.AddRequestData("vnp_CreateDate", createDate);
        vnPay.AddRequestData("vnp_CurrCode", "VND");
        vnPay.AddRequestData("vnp_IpAddr", ipAddress);
        vnPay.AddRequestData("vnp_Locale", "vn");
        vnPay.AddRequestData("vnp_OrderInfo", orderInfo);
        vnPay.AddRequestData("vnp_OrderType", "other");
        vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
        vnPay.AddRequestData("vnp_TxnRef", txnRef);
        vnPay.AddRequestData("vnp_ExpireDate", expireDate);

        var paymentUrl = vnPay.BuildPaymentUrl();

        return new PaymentUrlResponse
        {
            PaymentUrl = paymentUrl,
            OrderId = order.Id
        };
    }

    public async Task<bool> ProcessCallbackAsync(IQueryCollection query)
    {
        // Xác thực chữ ký VNPay
        if (!vnPay.ValidateSignature(query))
            return false;

        var txnRef = query["vnp_TxnRef"].ToString();
        var responseCode = query["vnp_ResponseCode"].ToString();
        var transactionId = query["vnp_TransactionNo"].ToString();

        if (!Guid.TryParseExact(txnRef, "N", out var orderId))
            return false;

        var order = await db.Orders
            .Include(o => o.Car)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return false;

        order.VnpayTransactionId = transactionId;
        order.VnpayResponseCode = responseCode;

        if (responseCode == "00") // Thanh toán thành công
        {
            order.PaymentStatus = PaymentStatus.Success;
            order.PaidAt = DateTime.UtcNow;

            // Cập nhật trạng thái xe sang Reserved
            order.Car.Status = CarStatus.Reserved;
            order.Car.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            order.PaymentStatus = PaymentStatus.Failed;
        }

        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return responseCode == "00";
    }

    public async Task<PagedResult<OrderResponse>> GetMyOrdersAsync(
        Guid userId, int page, int pageSize)
    {
        var query = db.Orders
            .Include(o => o.Car).ThenInclude(c => c.Images)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<OrderResponse>> GetAllOrdersAsync(int page, int pageSize)
    {
        var query = db.Orders
            .Include(o => o.Car).ThenInclude(c => c.Images)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId)
    {
        var order = await db.Orders
            .Include(o => o.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        return MapToResponse(order);
    }

    private static OrderResponse MapToResponse(Order o) => new()
    {
        Id = o.Id,
        CarId = o.CarId,
        CarTitle = $"{o.Car.Make} {o.Car.Model} {o.Car.Year}",
        CarPrimaryImage = o.Car.Images.FirstOrDefault(i => i.IsPrimary)?.Url
            ?? o.Car.Images.FirstOrDefault()?.Url,
        DepositAmount = o.DepositAmount,
        PaymentStatus = o.PaymentStatus.ToString(),
        VnpayTransactionId = o.VnpayTransactionId,
        PaidAt = o.PaidAt,
        CreatedAt = o.CreatedAt
    };
}
