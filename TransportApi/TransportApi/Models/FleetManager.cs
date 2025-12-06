using Microsoft.EntityFrameworkCore;
using TransportApi.Data;

namespace TransportApi.Models;

public class FleetManager
{
    private readonly FleetDbContext _db;

    public event EventHandler<TransportOrder>? OnNewOrderCreated;

    public FleetManager(FleetDbContext db)
    {
        _db = db;
    }

    public async Task<TransportOrder> CreateOrderAsync(int vehicleId, int driverId, string cargoDescription)
    {
        var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);
        var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);

        if (vehicle == null || driver == null)
            throw new InvalidOperationException("Vehicle or driver not found.");

        if (!vehicle.IsAvailable || !driver.IsAvailable)
            throw new InvalidOperationException("Vehicle or driver is not available.");

        var order = new TransportOrder
        {
            VehicleId = vehicleId,
            DriverId = driverId,
            Vehicle = vehicle,
            Driver = driver,
            CargoDescription = cargoDescription
        };

        order.Start(); // ustawia StartTime i dostępność

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        OnNewOrderCreated?.Invoke(this, order);

        return order;
    }

    public async Task CompleteOrderAsync(int orderId)
    {
        var order = await _db.Orders
            .Include(o => o.Vehicle)
            .Include(o => o.Driver)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new InvalidOperationException("Order not found.");

        if (order.IsCompleted)
            return;

        order.Complete();
        await _db.SaveChangesAsync();
    }
}