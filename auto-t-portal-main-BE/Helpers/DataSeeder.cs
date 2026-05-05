using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.Entities;

namespace auto_t_portal_main_BE.Helpers;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        // ── Users ──────────────────────────────────────────────
        var admin = new User
        {
            FullName = "Super Admin",
            Email = "admin@autot.vn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            PhoneNumber = "0901234567"
        };
        var staff = new User
        {
            FullName = "Nguyễn Văn Nhân Viên",
            Email = "staff@autot.vn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
            Role = UserRole.Staff,
            PhoneNumber = "0907654321"
        };
        var customer1 = new User
        {
            FullName = "Trần Thị Lan",
            Email = "lan@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
            Role = UserRole.Customer,
            PhoneNumber = "0912345678"
        };
        var customer2 = new User
        {
            FullName = "Lê Văn Mạnh",
            Email = "manh@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
            Role = UserRole.Customer,
            PhoneNumber = "0923456789"
        };

        db.Users.AddRange(admin, staff, customer1, customer2);
        await db.SaveChangesAsync();

        // ── Cars ───────────────────────────────────────────────
        var cars = new List<Car>
        {
            new Car
            {
                Make = "Toyota", Model = "Camry", Year = 2023,
                Price = 1_150_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "Toyota Camry 2023 - Sedan hạng D sang trọng, tiết kiệm nhiên liệu.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Trắng Ngọc Trai", VinNumber = "TYT2023CAM001"
            },
            new Car
            {
                Make = "Honda", Model = "CR-V", Year = 2023,
                Price = 1_050_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "Honda CR-V 2023 - SUV đa dụng, phù hợp gia đình.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Xanh Dương", VinNumber = "HND2023CRV001"
            },
            new Car
            {
                Make = "Mercedes-Benz", Model = "C200", Year = 2022,
                Price = 1_699_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "Mercedes-Benz C200 2022 - Đẳng cấp Đức, nội thất cao cấp.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Đen", VinNumber = "MBZ2022C200001"
            },
            new Car
            {
                Make = "VinFast", Model = "VF8", Year = 2023,
                Price = 979_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "VinFast VF8 2023 - SUV điện thuần túy, công nghệ hiện đại.",
                Mileage = 0, FuelType = "Điện", Transmission = "Tự động",
                SeatingCapacity = 7, Color = "Đỏ", VinNumber = "VNF2023VF8001"
            },
            new Car
            {
                Make = "Toyota", Model = "Fortuner", Year = 2021,
                Price = 950_000_000, Condition = CarCondition.Used,
                Status = CarStatus.Available,
                Description = "Toyota Fortuner 2021 đã qua sử dụng, còn rất mới, đi 25.000km.",
                Mileage = 25000, FuelType = "Dầu", Transmission = "Tự động",
                SeatingCapacity = 7, Color = "Bạc", VinNumber = "TYT2021FOR001"
            },
            new Car
            {
                Make = "BMW", Model = "320i", Year = 2022,
                Price = 1_599_000_000, Condition = CarCondition.New,
                Status = CarStatus.Reserved,
                Description = "BMW 320i 2022 - Sedan thể thao, động cơ mạnh mẽ.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Xám", VinNumber = "BMW2022320001"
            },
            new Car
            {
                Make = "Mazda", Model = "CX-5", Year = 2022,
                Price = 849_000_000, Condition = CarCondition.Used,
                Status = CarStatus.Available,
                Description = "Mazda CX-5 2022 đã qua sử dụng, đi 15.000km, còn bảo hành.",
                Mileage = 15000, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Đỏ Pha Lê", VinNumber = "MZD2022CX5001"
            },
            new Car
            {
                Make = "Hyundai", Model = "Tucson", Year = 2023,
                Price = 825_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "Hyundai Tucson 2023 - Thiết kế hiện đại, nhiều tính năng an toàn.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Trắng", VinNumber = "HYD2023TUC001"
            },
            new Car
            {
                Make = "Kia", Model = "Sorento", Year = 2022,
                Price = 1_099_000_000, Condition = CarCondition.New,
                Status = CarStatus.Sold,
                Description = "Kia Sorento 2022 - SUV 7 chỗ cao cấp.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 7, Color = "Đen", VinNumber = "KIA2022SOR001"
            },
            new Car
            {
                Make = "Porsche", Model = "Cayenne", Year = 2023,
                Price = 4_900_000_000, Condition = CarCondition.New,
                Status = CarStatus.Available,
                Description = "Porsche Cayenne 2023 - SUV siêu sang, hiệu suất vượt trội.",
                Mileage = 0, FuelType = "Xăng", Transmission = "Tự động",
                SeatingCapacity = 5, Color = "Vàng Gt", VinNumber = "POR2023CAY001"
            }
        };

        db.Cars.AddRange(cars);
        await db.SaveChangesAsync();

        // ── Appointments ───────────────────────────────────────
        var appointments = new List<Appointment>
        {
            new Appointment
            {
                UserId = customer1.Id,
                CarId = cars[0].Id,
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                Status = AppointmentStatus.Confirmed,
                Notes = "Khách muốn lái thử trên đường cao tốc"
            },
            new Appointment
            {
                UserId = customer2.Id,
                CarId = cars[1].Id,
                ScheduledAt = DateTime.UtcNow.AddDays(3),
                Status = AppointmentStatus.Pending,
                Notes = "Khách đi cùng gia đình"
            },
            new Appointment
            {
                UserId = customer1.Id,
                CarId = cars[3].Id,
                ScheduledAt = DateTime.UtcNow.AddDays(-5),
                Status = AppointmentStatus.Completed,
                Notes = "Đã lái thử, khách hài lòng"
            }
        };

        db.Appointments.AddRange(appointments);
        await db.SaveChangesAsync();

        // ── Orders (Đặt cọc) ───────────────────────────────────
        var order1 = new Order
        {
            UserId = customer1.Id,
            CarId = cars[5].Id,  // BMW 320i - Reserved
            DepositAmount = 100_000_000,
            PaymentStatus = PaymentStatus.Success,
            VnpayTransactionId = "VNP20260502001",
            VnpayResponseCode = "00",
            PaidAt = DateTime.UtcNow.AddDays(-3)
        };

        var order2 = new Order
        {
            UserId = customer2.Id,
            CarId = cars[8].Id,  // Kia Sorento - Sold
            DepositAmount = 150_000_000,
            PaymentStatus = PaymentStatus.Success,
            VnpayTransactionId = "VNP20260501001",
            VnpayResponseCode = "00",
            PaidAt = DateTime.UtcNow.AddDays(-10)
        };

        db.Orders.AddRange(order1, order2);
        await db.SaveChangesAsync();

        // ── Contracts ──────────────────────────────────────────
        var contract1 = new Contract
        {
            ContractNumber = "HD-20260502-ABC123",
            UserId = customer1.Id,
            CarId = cars[5].Id,
            OrderId = order1.Id,
            TotalAmount = 1_599_000_000,
            DepositAmount = 100_000_000,
            RemainingAmount = 1_499_000_000,
            Status = ContractStatus.Signed,
            SignedAt = DateTime.UtcNow.AddDays(-2),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(5),
            Terms = "Thanh toán phần còn lại trước khi nhận xe. Xe được bảo hành 3 năm.",
            Notes = "Khách đã thanh toán cọc qua VNPay"
        };

        var contract2 = new Contract
        {
            ContractNumber = "HD-20260501-DEF456",
            UserId = customer2.Id,
            CarId = cars[8].Id,
            OrderId = order2.Id,
            TotalAmount = 1_099_000_000,
            DepositAmount = 150_000_000,
            RemainingAmount = 949_000_000,
            Status = ContractStatus.Completed,
            SignedAt = DateTime.UtcNow.AddDays(-9),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(-5),
            Terms = "Đã hoàn tất giao dịch.",
            Notes = "Hợp đồng hoàn thành"
        };

        db.Contracts.AddRange(contract1, contract2);
        await db.SaveChangesAsync();

        // ── Deliveries ─────────────────────────────────────────
        var delivery = new Delivery
        {
            ContractId = contract2.Id,
            UserId = customer2.Id,
            CarId = cars[8].Id,
            Status = DeliveryStatus.Completed,
            ScheduledAt = DateTime.UtcNow.AddDays(-6),
            DeliveredAt = DateTime.UtcNow.AddDays(-5),
            DeliveryAddress = "123 Nguyễn Huệ, Q.1, TP.HCM",
            ReceiverName = "Lê Văn Mạnh",
            ReceiverPhone = "0923456789",
            HasOwnershipDoc = true,
            HasInsurance = true,
            HasMaintenanceBook = true,
            HasSpareKey = true,
            StaffNotes = "Bàn giao đầy đủ, khách hài lòng"
        };

        db.Deliveries.Add(delivery);
        await db.SaveChangesAsync();

        Console.WriteLine("✅ Seed data completed:");
        Console.WriteLine("   👤 Admin:    admin@autot.vn / Admin@123");
        Console.WriteLine("   👤 Staff:    staff@autot.vn / Staff@123");
        Console.WriteLine("   👤 Customer: lan@gmail.com / Customer@123");
        Console.WriteLine("   👤 Customer: manh@gmail.com / Customer@123");
        Console.WriteLine($"   🚗 {cars.Count} xe mẫu đã được tạo");
        Console.WriteLine($"   📅 {appointments.Count} lịch hẹn mẫu");
        Console.WriteLine($"   💳 2 đơn đặt cọc mẫu");
        Console.WriteLine($"   📄 2 hợp đồng mẫu");
        Console.WriteLine($"   🚚 1 bàn giao xe mẫu");
    }
}
