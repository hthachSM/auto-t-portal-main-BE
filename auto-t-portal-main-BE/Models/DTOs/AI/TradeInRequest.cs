using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.AI;

public class TradeInRequest
{
    [Required] public string Make { get; set; } = default!;
    [Required] public string Model { get; set; } = default!;
    [Required] public int Year { get; set; }
    [Required] public int Mileage { get; set; }
    public string? Condition { get; set; }
    public string? Color { get; set; }
}
