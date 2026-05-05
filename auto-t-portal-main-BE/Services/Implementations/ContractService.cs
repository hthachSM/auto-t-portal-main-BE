using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Contract;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class ContractService(AppDbContext db) : IContractService
{
    public async Task<ContractResponse> CreateAsync(CreateContractRequest request)
    {
        var user = await db.Users.FindAsync(request.UserId)
            ?? throw new KeyNotFoundException("Không tìm thấy khách hàng.");

        var car = await db.Cars.Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == request.CarId)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        if (car.Status == CarStatus.Sold)
            throw new InvalidOperationException("Xe này đã được bán.");

        // Tự động tạo số hợp đồng
        var contractNumber = $"HD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var remaining = request.TotalAmount - request.DepositAmount;

        var contract = new Contract
        {
            ContractNumber = contractNumber,
            UserId = request.UserId,
            CarId = request.CarId,
            OrderId = request.OrderId,
            TotalAmount = request.TotalAmount,
            DepositAmount = request.DepositAmount,
            RemainingAmount = remaining,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Terms = request.Terms ?? DefaultTerms(),
            Notes = request.Notes,
            Status = ContractStatus.Draft
        };

        db.Contracts.Add(contract);

        // Cập nhật trạng thái xe sang Reserved
        car.Status = CarStatus.Reserved;
        car.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        contract.User = user;
        contract.Car = car;
        return MapToResponse(contract);
    }

    public async Task<ContractResponse> UpdateStatusAsync(Guid id, UpdateContractStatusRequest request)
    {
        var contract = await db.Contracts
            .Include(c => c.User)
            .Include(c => c.Car).ThenInclude(c => c.Images)
            .Include(c => c.Delivery)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        contract.Status = request.Status;
        if (request.Notes != null) contract.Notes = request.Notes;

        if (request.Status == ContractStatus.Signed)
            contract.SignedAt = DateTime.UtcNow;

        if (request.Status == ContractStatus.Completed)
        {
            // Xe đã bán hoàn toàn
            contract.Car.Status = CarStatus.Sold;
            contract.Car.UpdatedAt = DateTime.UtcNow;
        }

        if (request.Status == ContractStatus.Cancelled)
        {
            // Xe về trạng thái available
            contract.Car.Status = CarStatus.Available;
            contract.Car.UpdatedAt = DateTime.UtcNow;
        }

        contract.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapToResponse(contract);
    }

    public async Task<ContractResponse> GetByIdAsync(Guid id)
    {
        var contract = await db.Contracts
            .Include(c => c.User)
            .Include(c => c.Car).ThenInclude(c => c.Images)
            .Include(c => c.Delivery)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        return MapToResponse(contract);
    }

    public async Task<List<ContractResponse>> GetAllAsync()
    {
        var contracts = await db.Contracts
            .Include(c => c.User)
            .Include(c => c.Car).ThenInclude(c => c.Images)
            .Include(c => c.Delivery)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return contracts.Select(MapToResponse).ToList();
    }

    public async Task<List<ContractResponse>> GetByUserIdAsync(Guid userId)
    {
        var contracts = await db.Contracts
            .Include(c => c.User)
            .Include(c => c.Car).ThenInclude(c => c.Images)
            .Include(c => c.Delivery)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return contracts.Select(MapToResponse).ToList();
    }

    private static string DefaultTerms() => """
        1. Bên mua đồng ý thanh toán đầy đủ số tiền còn lại trước khi nhận xe.
        2. Bên bán cam kết bàn giao xe đúng tình trạng như mô tả.
        3. Xe được bảo hành theo chính sách của nhà sản xuất.
        4. Hợp đồng có hiệu lực kể từ ngày ký.
        5. Mọi tranh chấp được giải quyết theo pháp luật Việt Nam.
        """;

    private static ContractResponse MapToResponse(Contract c) => new()
    {
        Id = c.Id,
        ContractNumber = c.ContractNumber,
        UserId = c.UserId,
        CustomerName = c.User.FullName,
        CustomerEmail = c.User.Email,
        CustomerPhone = c.User.PhoneNumber,
        CarId = c.CarId,
        CarTitle = $"{c.Car.Make} {c.Car.Model} {c.Car.Year}",
        CarPrimaryImage = c.Car.Images.FirstOrDefault(i => i.IsPrimary)?.Url
            ?? c.Car.Images.FirstOrDefault()?.Url,
        TotalAmount = c.TotalAmount,
        DepositAmount = c.DepositAmount,
        RemainingAmount = c.RemainingAmount,
        Status = c.Status.ToString(),
        SignedAt = c.SignedAt,
        ExpectedDeliveryDate = c.ExpectedDeliveryDate,
        Terms = c.Terms,
        Notes = c.Notes,
        CreatedAt = c.CreatedAt,
        HasDelivery = c.Delivery != null
    };
}
