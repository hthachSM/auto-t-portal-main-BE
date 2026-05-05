using auto_t_portal_main_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarImage> CarImages => Set<CarImage>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Car>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Contract>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Delivery>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<CarImage>().HasQueryFilter(e => !e.Car.IsDeleted);

        builder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(200).IsRequired();
        });

        builder.Entity<Car>(e =>
        {
            e.Property(c => c.Price).HasColumnType("decimal(18,2)");
            e.Property(c => c.Make).HasMaxLength(100).IsRequired();
            e.Property(c => c.Model).HasMaxLength(100).IsRequired();
            e.HasMany(c => c.Images)
             .WithOne(i => i.Car)
             .HasForeignKey(i => i.CarId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(e =>
        {
            e.Property(o => o.DepositAmount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Contract>(e =>
        {
            e.Property(c => c.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(c => c.DepositAmount).HasColumnType("decimal(18,2)");
            e.Property(c => c.RemainingAmount).HasColumnType("decimal(18,2)");
            e.HasIndex(c => c.ContractNumber).IsUnique();
            e.HasOne(c => c.Delivery)
             .WithOne(d => d.Contract)
             .HasForeignKey<Delivery>(d => d.ContractId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
