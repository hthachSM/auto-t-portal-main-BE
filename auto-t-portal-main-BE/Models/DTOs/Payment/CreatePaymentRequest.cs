using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Payment;

public class CreatePaymentRequest
{
    [Required]
    public Guid CarId { get; set; }

    [Required, Range(1, double.MaxValue)]
    public decimal DepositAmount { get; set; }

    public string? OrderInfo { get; set; }
}
