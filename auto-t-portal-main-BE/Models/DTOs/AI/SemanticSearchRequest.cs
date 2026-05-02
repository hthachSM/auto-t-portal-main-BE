using System.ComponentModel.DataAnnotations;

namespace auto_t_portal_main_BE.Models.DTOs.AI;

public class SemanticSearchRequest
{
    [Required]
    public string Query { get; set; } = default!;

    public int TopK { get; set; } = 5;
}
