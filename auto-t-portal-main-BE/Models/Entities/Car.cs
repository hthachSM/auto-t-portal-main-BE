using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Models.Entities;

public enum CarStatus { Available = 1, Reserved = 2, Sold = 3 }
public enum CarCondition { New = 1, Used = 2 }

public class Car : BaseEntity
{
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public CarCondition Condition { get; set; }
    public CarStatus Status { get; set; } = CarStatus.Available;
    public string? Description { get; set; }
    public int Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int SeatingCapacity { get; set; }
    public string? Color { get; set; }
    public string? VinNumber { get; set; }
    public string? EmbeddingJson { get; set; }

    public ICollection<CarImage> Images { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}