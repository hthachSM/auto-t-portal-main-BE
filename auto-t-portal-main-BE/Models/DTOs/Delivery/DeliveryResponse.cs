namespace auto_t_portal_main_BE.Models.DTOs.Delivery;

public class DeliveryResponse
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string ContractNumber { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string CarTitle { get; set; } = default!;
    public string? CarPrimaryImage { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? Notes { get; set; }
    public string? StaffNotes { get; set; }
    public bool HasOwnershipDoc { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasMaintenanceBook { get; set; }
    public bool HasSpareKey { get; set; }
    public DateTime CreatedAt { get; set; }
}
