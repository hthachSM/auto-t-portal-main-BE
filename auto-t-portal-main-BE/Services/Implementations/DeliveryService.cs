using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Delivery;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class DeliveryService(AppDbContext db) : IDeliveryService
{
    public async Task<DeliveryResponse> CreateAsync(CreateDeliveryRequest request)
    {
        var contract = await db.Contracts
            .Include(c => c.User)
            .Include(c => c.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == request.ContractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (contract.Status != ContractStatus.Signed)
            throw new InvalidOperationException("Hợp đồng phải được ký trước khi tạo lịch bàn giao.");

        var existing = await db.Deliveries
            .AnyAsync(d => d.ContractId == request.ContractId);
        if (existing)
            throw new InvalidOperationException("Hợp đồng này đã có lịch bàn giao.");

        var delivery = new Delivery
        {
            ContractId = request.ContractId,
            UserId = contract.UserId,
            CarId = contract.CarId,
            ScheduledAt = request.ScheduledAt,
            DeliveryAddress = request.DeliveryAddress,
            ReceiverName = request.ReceiverName ?? contract.User.FullName,
            ReceiverPhone = request.ReceiverPhone ?? contract.User.PhoneNumber,
            Notes = request.Notes,
            Status = DeliveryStatus.Scheduled
        };

        db.Deliveries.Add(delivery);
        await db.SaveChangesAsync();

        delivery.Contract = contract;
        delivery.User = contract.User;
        delivery.Car = contract.Car;
        return MapToResponse(delivery);
    }

    public async Task<DeliveryResponse> CompleteAsync(Guid id, CompleteDeliveryRequest request)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Contract)
            .Include(d => d.User)
            .Include(d => d.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch bàn giao.");

        delivery.HasOwnershipDoc = request.HasOwnershipDoc;
        delivery.HasInsurance = request.HasInsurance;
        delivery.HasMaintenanceBook = request.HasMaintenanceBook;
        delivery.HasSpareKey = request.HasSpareKey;
        delivery.StaffNotes = request.StaffNotes;
        delivery.Status = DeliveryStatus.Completed;
        delivery.DeliveredAt = DateTime.UtcNow;
        delivery.UpdatedAt = DateTime.UtcNow;

        // Cập nhật hợp đồng và xe sang trạng thái hoàn thành
        delivery.Contract.Status = ContractStatus.Completed;
        delivery.Contract.UpdatedAt = DateTime.UtcNow;
        delivery.Car.Status = CarStatus.Sold;
        delivery.Car.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return MapToResponse(delivery);
    }

    public async Task<DeliveryResponse> GetByIdAsync(Guid id)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Contract)
            .Include(d => d.User)
            .Include(d => d.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch bàn giao.");

        return MapToResponse(delivery);
    }

    public async Task<DeliveryResponse> GetByContractIdAsync(Guid contractId)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Contract)
            .Include(d => d.User)
            .Include(d => d.Car).ThenInclude(c => c.Images)
            .FirstOrDefaultAsync(d => d.ContractId == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy lịch bàn giao.");

        return MapToResponse(delivery);
    }

    public async Task<List<DeliveryResponse>> GetAllAsync()
    {
        var deliveries = await db.Deliveries
            .Include(d => d.Contract)
            .Include(d => d.User)
            .Include(d => d.Car).ThenInclude(c => c.Images)
            .OrderByDescending(d => d.ScheduledAt)
            .ToListAsync();

        return deliveries.Select(MapToResponse).ToList();
    }

    private static DeliveryResponse MapToResponse(Delivery d) => new()
    {
        Id = d.Id,
        ContractId = d.ContractId,
        ContractNumber = d.Contract.ContractNumber,
        CustomerName = d.User.FullName,
        CarTitle = $"{d.Car.Make} {d.Car.Model} {d.Car.Year}",
        CarPrimaryImage = d.Car.Images.FirstOrDefault(i => i.IsPrimary)?.Url
            ?? d.Car.Images.FirstOrDefault()?.Url,
        Status = d.Status.ToString(),
        ScheduledAt = d.ScheduledAt,
        DeliveredAt = d.DeliveredAt,
        DeliveryAddress = d.DeliveryAddress,
        ReceiverName = d.ReceiverName,
        ReceiverPhone = d.ReceiverPhone,
        Notes = d.Notes,
        StaffNotes = d.StaffNotes,
        HasOwnershipDoc = d.HasOwnershipDoc,
        HasInsurance = d.HasInsurance,
        HasMaintenanceBook = d.HasMaintenanceBook,
        HasSpareKey = d.HasSpareKey,
        CreatedAt = d.CreatedAt
    };
}
