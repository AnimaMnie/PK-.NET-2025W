using Microsoft.EntityFrameworkCore;
using TransportApi.Models;

namespace TransportApi.Data;

public class FleetDbContext : DbContext
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Truck> Trucks => Set<Truck>();
    public DbSet<Van> Vans => Set<Van>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<TransportOrder> Orders => Set<TransportOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>()
            .HasDiscriminator<string>("VehicleType")
            .HasValue<Truck>("Truck")
            .HasValue<Van>("Van");

        modelBuilder.Entity<TransportOrder>()
            .HasOne(o => o.Vehicle)
            .WithMany()
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransportOrder>()
            .HasOne(o => o.Driver)
            .WithMany()
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}