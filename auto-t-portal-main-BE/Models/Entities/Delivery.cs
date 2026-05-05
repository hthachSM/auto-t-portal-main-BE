namespace auto_t_portal_main_BE.Models.Entities;

public enum DeliveryStatus
{
    Pending = 1,      // Chờ bàn giao
    Scheduled = 2,    // Đã lên lịch
    Completed = 3,    // Đã bàn giao
    Cancelled = 4     // Hủy
}

public class Delivery : BaseEntity
{
    public Guid ContractId { get; set; }
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public DateTime? ScheduledAt { get; set; }              // Ngày giao dự kiến
    public DateTime? DeliveredAt { get; set; }              // Ngày giao thực tế

    public string? DeliveryAddress { get; set; }            // Địa điểm bàn giao
    public string? ReceiverName { get; set; }               // Người nhận
    public string? ReceiverPhone { get; set; }
    public string? Notes { get; set; }
    public string? StaffNotes { get; set; }                 // Ghi chú nhân viên

    // Checklist bàn giao
    public bool HasOwnershipDoc { get; set; }               // Giấy tờ xe
    public bool HasInsurance { get; set; }                  // Bảo hiểm
    public bool HasMaintenanceBook { get; set; }            // Sách bảo dưỡng
    public bool HasSpareKey { get; set; }                   // Chìa khóa dự phòng

    public Contract Contract { get; set; } = default!;
    public User User { get; set; } = default!;
    public Car Car { get; set; } = default!;
}
