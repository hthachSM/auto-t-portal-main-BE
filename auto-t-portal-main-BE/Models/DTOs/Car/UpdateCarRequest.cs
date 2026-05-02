using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Car;

public class UpdateCarRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public decimal? Price { get; set; }
    public CarCondition? Condition { get; set; }
    public CarStatus? Status { get; set; }
    public string? Description { get; set; }
    public int? Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int? SeatingCapacity { get; set; }
    public string? Color { get; set; }
    public string? VinNumber { get; set; }
}
