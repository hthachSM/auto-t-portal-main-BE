using auto_t_portal_main_BE.Models.DTOs.AI;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IAiService
{
    Task<SemanticSearchResponse> SemanticSearchAsync(SemanticSearchRequest request);
    Task<TradeInResponse> EstimateTradeInAsync(TradeInRequest request);
    Task GenerateEmbeddingsForAllCarsAsync();
}
