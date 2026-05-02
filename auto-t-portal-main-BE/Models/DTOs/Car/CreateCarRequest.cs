using System.ComponentModel.DataAnnotations;
using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Car;

public class CreateCarRequest
{
    [Required] public string Make { get; set; } = default!;
    [Required] public string Model { get; set; } = default!;
    [Required] public int Year { get; set; }
    [Required, Range(0, double.MaxValue)] public decimal Price { get; set; }
    [Required] public CarCondition Condition { get; set; }
    public string? Description { get; set; }
    public int Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int SeatingCapacity { get; set; }
    public string? Color { get; set; }
    public string? VinNumber { get; set; }
}
