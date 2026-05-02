namespace auto_t_portal_main_BE.Models.DTOs.Report;

public class DashboardSummaryResponse
{
    public int TotalCars { get; set; }
    public int AvailableCars { get; set; }
    public int ReservedCars { get; set; }
    public int SoldCars { get; set; }
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
}
