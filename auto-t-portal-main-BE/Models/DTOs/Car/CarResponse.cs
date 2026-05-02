using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Car;

public class CarResponse
{
    public Guid Id { get; set; }
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Condition { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? Description { get; set; }
    public int Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int SeatingCapacity { get; set; }
    public string? Color { get; set; }
    public string? VinNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CarImageResponse> Images { get; set; } = [];
}

public class CarImageResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = default!;
    public bool IsPrimary { get; set; }
}
