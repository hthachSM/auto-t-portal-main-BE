using auto_t_portal_main_BE.Models.DTOs.Report;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IReportService
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync();
    Task<List<RevenueByMonthResponse>> GetRevenueByMonthAsync(int year);
    Task<List<TopCarResponse>> GetTopCarsAsync(int topN = 10);
    Task<List<CarStatusDistributionResponse>> GetCarStatusDistributionAsync();
    Task<List<AppointmentTrendResponse>> GetAppointmentTrendAsync(int days = 30);
}
