namespace auto_t_portal_main_BE.Models.DTOs.AI;

public class SemanticSearchResponse
{
    public string Answer { get; set; } = default!;
    public List<CarSearchResult> RelatedCars { get; set; } = [];
}

public class CarSearchResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public decimal Price { get; set; }
    public string Condition { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? PrimaryImage { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int SeatingCapacity { get; set; }
    public double SimilarityScore { get; set; }
}
