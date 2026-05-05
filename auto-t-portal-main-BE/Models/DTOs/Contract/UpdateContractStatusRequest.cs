using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Models.DTOs.Contract;

public class UpdateContractStatusRequest
{
    public ContractStatus Status { get; set; }
    public string? Notes { get; set; }
}
