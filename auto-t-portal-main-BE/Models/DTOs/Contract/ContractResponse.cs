namespace auto_t_portal_main_BE.Models.DTOs.Contract;

public class ContractResponse
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = default!;
    public Guid UserId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string? CustomerPhone { get; set; }
    public Guid CarId { get; set; }
    public string CarTitle { get; set; } = default!;
    public string? CarPrimaryImage { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? SignedAt { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasDelivery { get; set; }
}
