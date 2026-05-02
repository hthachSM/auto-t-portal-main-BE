using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Models.Entities;

public enum PaymentStatus { Pending = 1, Success = 2, Failed = 3, Refunded = 4 }

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public decimal DepositAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? VnpayTransactionId { get; set; }
    public string? VnpayResponseCode { get; set; }
    public DateTime? PaidAt { get; set; }
    public User User { get; set; } = default!;
    public Car Car { get; set; } = default!;
}