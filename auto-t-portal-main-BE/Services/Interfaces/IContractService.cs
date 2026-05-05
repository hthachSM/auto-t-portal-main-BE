using auto_t_portal_main_BE.Models.DTOs.Contract;

namespace auto_t_portal_main_BE.Services.Interfaces;

public interface IContractService
{
    Task<ContractResponse> CreateAsync(CreateContractRequest request);
    Task<ContractResponse> UpdateStatusAsync(Guid id, UpdateContractStatusRequest request);
    Task<ContractResponse> GetByIdAsync(Guid id);
    Task<List<ContractResponse>> GetAllAsync();
    Task<List<ContractResponse>> GetByUserIdAsync(Guid userId);
}
