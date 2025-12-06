namespace TransportApi.Models;

public static class VehicleExtensions
{
    public static IQueryable<Vehicle> GetAvailableVehicles(this IQueryable<Vehicle> vehicles)
        => vehicles.Where(v => v.IsAvailable);

    public static IEnumerable<Vehicle> GetAvailableVehicles(this IEnumerable<Vehicle> vehicles)
        => vehicles.Where(v => v.IsAvailable);
}