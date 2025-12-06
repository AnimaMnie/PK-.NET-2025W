using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;
using Xunit;


namespace TransportApi.Tests;

public class FleetTests
{
    private FleetDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FleetDbContext(options);
    }

    [Fact]
    public async Task Can_Add_Vehicle_And_Driver()
    {
        using var db = CreateInMemoryContext();

        var truck = new Truck
        {
            RegistrationNumber = "WX12345",
            Model = "Volvo FH",
            TrailerLength = 13.6
        };
        db.Vehicles.Add(truck);

        var driver = new Driver
        {
            FirstName = "Jan",
            LastName = "Kowalski"
        };
        db.Drivers.Add(driver);

        await db.SaveChangesAsync();

        Assert.Equal(1, db.Vehicles.Count());
        Assert.Equal(1, db.Drivers.Count());
    }

    [Fact]
    public async Task Can_Create_Order_And_Reserve_Resources()
    {
        using var db = CreateInMemoryContext();

        var truck = new Truck
        {
            RegistrationNumber = "WX12345",
            Model = "Volvo FH",
            TrailerLength = 13.6
        };
        db.Vehicles.Add(truck);

        var driver = new Driver
        {
            FirstName = "Jan",
            LastName = "Kowalski"
        };
        db.Drivers.Add(driver);

        await db.SaveChangesAsync();

        var manager = new FleetManager(db);

        var order = await manager.CreateOrderAsync(truck.Id, driver.Id, "Płyty gk");

        Assert.False(order.Vehicle!.IsAvailable);
        Assert.False(order.Driver!.IsAvailable);
        Assert.NotEqual(default, order.StartTime);
        Assert.False(order.IsCompleted);
    }

    [Fact]
    public async Task Complete_Order_Releases_Resources()
    {
        using var db = CreateInMemoryContext();

        var truck = new Truck
        {
            RegistrationNumber = "WX12345",
            Model = "Volvo FH",
            TrailerLength = 13.6
        };
        db.Vehicles.Add(truck);

        var driver = new Driver
        {
            FirstName = "Jan",
            LastName = "Kowalski"
        };
        db.Drivers.Add(driver);

        await db.SaveChangesAsync();

        var manager = new FleetManager(db);
        var order = await manager.CreateOrderAsync(truck.Id, driver.Id, "Płyty gk");

        await manager.CompleteOrderAsync(order.Id);
        var refreshed = await db.Orders
            .Include(o => o.Vehicle)
            .Include(o => o.Driver)
            .FirstAsync(o => o.Id == order.Id);

        Assert.True(refreshed.IsCompleted);
        Assert.True(refreshed.Vehicle!.IsAvailable);
        Assert.True(refreshed.Driver!.IsAvailable);
        Assert.NotNull(refreshed.EndTime);
    }

    [Fact]
    public async Task OnNewOrderCreated_Event_Is_Raised()
    {
        using var db = CreateInMemoryContext();

        var truck = new Truck
        {
            RegistrationNumber = "WX12345",
            Model = "Volvo FH",
            TrailerLength = 13.6
        };
        db.Vehicles.Add(truck);

        var driver = new Driver
        {
            FirstName = "Jan",
            LastName = "Kowalski"
        };
        db.Drivers.Add(driver);

        await db.SaveChangesAsync();

        var manager = new FleetManager(db);

        bool eventRaised = false;
        TransportOrder? raisedOrder = null;

        manager.OnNewOrderCreated += (sender, order) =>
        {
            eventRaised = true;
            raisedOrder = order;
        };

        var orderCreated = await manager.CreateOrderAsync(truck.Id, driver.Id, "Płyty gk");

        Assert.True(eventRaised);
        Assert.NotNull(raisedOrder);
        Assert.Equal(orderCreated.Id, raisedOrder!.Id);
    }

    [Fact]
    public void GetAvailableVehicles_Returns_Only_Available()
    {
        var vehicles = new Vehicle[]
        {
            new Truck { Id = 1, RegistrationNumber = "A", Model = "M1", TrailerLength = 10, IsAvailable = true },
            new Van   { Id = 2, RegistrationNumber = "B", Model = "M2", CargoVolume = 5, IsAvailable = false },
            new Truck { Id = 3, RegistrationNumber = "C", Model = "M3", TrailerLength = 12, IsAvailable = true },
        };

        var available = vehicles.GetAvailableVehicles().ToList();

        Assert.Equal(2, available.Count);
        Assert.DoesNotContain(available, v => v.Id == 2);
    }
}
