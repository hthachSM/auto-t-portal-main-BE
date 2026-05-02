namespace auto_t_portal_main_BE.Models.DTOs.Payment;

public class PaymentUrlResponse
{
    public string PaymentUrl { get; set; } = default!;
    public Guid OrderId { get; set; }
}
