namespace auto_t_portal_main_BE.Models.DTOs.Payment;

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string CarTitle { get; set; } = default!;
    public string? CarPrimaryImage { get; set; }
    public decimal DepositAmount { get; set; }
    public string PaymentStatus { get; set; } = default!;
    public string? VnpayTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
