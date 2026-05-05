using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.Delivery;

public class CreateDeliveryRequest
{
    [Required] public Guid ContractId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? Notes { get; set; }
}
