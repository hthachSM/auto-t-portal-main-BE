namespace auto_t_portal_main_BE.Models.DTOs.AI;

public class TradeInResponse
{
    public decimal EstimatedMinPrice { get; set; }
    public decimal EstimatedMaxPrice { get; set; }
    public decimal SuggestedPrice { get; set; }
    public string Explanation { get; set; } = default!;
    public List<string> Factors { get; set; } = [];
}
