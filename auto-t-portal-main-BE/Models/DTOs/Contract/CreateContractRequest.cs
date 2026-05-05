using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Contract;

public class CreateContractRequest
{
    [Required] public Guid UserId { get; set; }
    [Required] public Guid CarId { get; set; }
    public Guid? OrderId { get; set; }
    [Required, Range(0, double.MaxValue)] public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
}
