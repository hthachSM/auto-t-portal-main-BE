using Microsoft.AspNetCore.Mvc;

namespace auto_t_portal_main_BE.Models.Entities;

public class CarImage : BaseEntity
{
    public Guid CarId { get; set; }
    public string Url { get; set; } = default!;
    public bool IsPrimary { get; set; } = false;
    public Car Car { get; set; } = default!;
}