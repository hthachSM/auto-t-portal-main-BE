namespace auto_t_portal_main_BE.Models.Entities;

public enum ContractStatus
{
    Draft = 1,        // Nháp
    Signed = 2,       // Đã ký
    Completed = 3,    // Hoàn thành
    Cancelled = 4     // Hủy
}

public class Contract : BaseEntity
{
    public string ContractNumber { get; set; } = default!;  // Số hợp đồng
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public Guid? OrderId { get; set; }                      // Liên kết đặt cọc

    public decimal TotalAmount { get; set; }                // Giá bán cuối
    public decimal DepositAmount { get; set; }              // Tiền cọc đã thanh toán
    public decimal RemainingAmount { get; set; }            // Còn lại cần thanh toán

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    public DateTime? SignedAt { get; set; }                 // Ngày ký
    public DateTime? ExpectedDeliveryDate { get; set; }     // Ngày giao xe dự kiến

    public string? Terms { get; set; }                      // Điều khoản hợp đồng
    public string? Notes { get; set; }
    public string? StaffId { get; set; }                    // Nhân viên phụ trách

    public User User { get; set; } = default!;
    public Car Car { get; set; } = default!;
    public Order? Order { get; set; }
    public Delivery? Delivery { get; set; }
}
