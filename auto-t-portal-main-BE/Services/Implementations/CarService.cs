using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.Car;
using auto_t_portal_main_BE.Models.Entities;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class CarService(AppDbContext db, IWebHostEnvironment env) : ICarService
{
    public async Task<CarResponse> CreateAsync(CreateCarRequest request)
    {
        var car = new Car
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            Condition = request.Condition,
            Description = request.Description,
            Mileage = request.Mileage,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            SeatingCapacity = request.SeatingCapacity,
            Color = request.Color,
            VinNumber = request.VinNumber,
            Status = CarStatus.Available
        };

        db.Cars.Add(car);
        await db.SaveChangesAsync();
        return MapToResponse(car);
    }

    public async Task<CarResponse> UpdateAsync(Guid id, UpdateCarRequest request)
    {
        var car = await db.Cars.Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        if (request.Make != null) car.Make = request.Make;
        if (request.Model != null) car.Model = request.Model;
        if (request.Year.HasValue) car.Year = request.Year.Value;
        if (request.Price.HasValue) car.Price = request.Price.Value;
        if (request.Condition.HasValue) car.Condition = request.Condition.Value;
        if (request.Status.HasValue) car.Status = request.Status.Value;
        if (request.Description != null) car.Description = request.Description;
        if (request.Mileage.HasValue) car.Mileage = request.Mileage.Value;
        if (request.FuelType != null) car.FuelType = request.FuelType;
        if (request.Transmission != null) car.Transmission = request.Transmission;
        if (request.SeatingCapacity.HasValue) car.SeatingCapacity = request.SeatingCapacity.Value;
        if (request.Color != null) car.Color = request.Color;
        if (request.VinNumber != null) car.VinNumber = request.VinNumber;

        car.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapToResponse(car);
    }

    public async Task DeleteAsync(Guid id)
    {
        var car = await db.Cars.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        car.IsDeleted = true;
        car.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<CarResponse> GetByIdAsync(Guid id)
    {
        var car = await db.Cars.Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        return MapToResponse(car);
    }

    public async Task<PagedResult<CarResponse>> GetAllAsync(CarFilterRequest filter)
    {
        var query = db.Cars.Include(c => c.Images).AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(filter.Make))
            query = query.Where(c => c.Make.Contains(filter.Make));
        if (!string.IsNullOrEmpty(filter.Model))
            query = query.Where(c => c.Model.Contains(filter.Model));
        if (filter.YearFrom.HasValue)
            query = query.Where(c => c.Year >= filter.YearFrom.Value);
        if (filter.YearTo.HasValue)
            query = query.Where(c => c.Year <= filter.YearTo.Value);
        if (filter.PriceFrom.HasValue)
            query = query.Where(c => c.Price >= filter.PriceFrom.Value);
        if (filter.PriceTo.HasValue)
            query = query.Where(c => c.Price <= filter.PriceTo.Value);
        if (filter.Condition.HasValue)
            query = query.Where(c => c.Condition == filter.Condition.Value);
        if (filter.Status.HasValue)
            query = query.Where(c => c.Status == filter.Status.Value);
        if (!string.IsNullOrEmpty(filter.FuelType))
            query = query.Where(c => c.FuelType == filter.FuelType);
        if (!string.IsNullOrEmpty(filter.Transmission))
            query = query.Where(c => c.Transmission == filter.Transmission);

        // Sorting
        query = (filter.SortBy?.ToLower(), filter.SortOrder?.ToLower()) switch
        {
            ("price", "desc") => query.OrderByDescending(c => c.Price),
            ("price", _)      => query.OrderBy(c => c.Price),
            ("year", "desc")  => query.OrderByDescending(c => c.Year),
            ("year", _)       => query.OrderBy(c => c.Year),
            ("createdat", "asc") => query.OrderBy(c => c.CreatedAt),
            _                 => query.OrderByDescending(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<CarResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<CarResponse> AddImageAsync(Guid carId, IFormFile file)
    {
        var car = await db.Cars.Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == carId)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        // Lưu file vào wwwroot/uploads/cars
        var uploadsDir = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads", "cars");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var image = new CarImage
        {
            CarId = carId,
            Url = $"/uploads/cars/{fileName}",
            IsPrimary = !car.Images.Any() // Ảnh đầu tiên là primary
        };

        db.CarImages.Add(image);
        await db.SaveChangesAsync();

        car.Images.Add(image);
        return MapToResponse(car);
    }

    public async Task DeleteImageAsync(Guid imageId)
    {
        var image = await db.CarImages.FindAsync(imageId)
            ?? throw new KeyNotFoundException("Không tìm thấy ảnh.");

        // Xóa file vật lý
        var filePath = Path.Combine(
            env.WebRootPath ?? "wwwroot",
            image.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(filePath)) File.Delete(filePath);

        db.CarImages.Remove(image);
        await db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        var car = await db.Cars.FindAsync(id)
            ?? throw new KeyNotFoundException("Không tìm thấy xe.");

        if (!Enum.TryParse<CarStatus>(status, true, out var carStatus))
            throw new InvalidOperationException("Trạng thái không hợp lệ.");

        car.Status = carStatus;
        car.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    private static CarResponse MapToResponse(Car car) => new()
    {
        Id = car.Id,
        Make = car.Make,
        Model = car.Model,
        Year = car.Year,
        Price = car.Price,
        Condition = car.Condition.ToString(),
        Status = car.Status.ToString(),
        Description = car.Description,
        Mileage = car.Mileage,
        FuelType = car.FuelType,
        Transmission = car.Transmission,
        SeatingCapacity = car.SeatingCapacity,
        Color = car.Color,
        VinNumber = car.VinNumber,
        CreatedAt = car.CreatedAt,
        Images = car.Images.Select(i => new CarImageResponse
        {
            Id = i.Id,
            Url = i.Url,
            IsPrimary = i.IsPrimary
        }).ToList()
    };
}
