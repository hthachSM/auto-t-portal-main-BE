using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Car;

public class CarFilterRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public CarCondition? Condition { get; set; }
    public CarStatus? Status { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }      // price, year, createdAt
    public string? SortOrder { get; set; }   // asc, desc
}
