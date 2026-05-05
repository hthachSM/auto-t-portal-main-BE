namespace auto_t_portal_main_BE.Models.DTOs.Delivery;

public class CompleteDeliveryRequest
{
    public bool HasOwnershipDoc { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasMaintenanceBook { get; set; }
    public bool HasSpareKey { get; set; }
    public string? StaffNotes { get; set; }
}
