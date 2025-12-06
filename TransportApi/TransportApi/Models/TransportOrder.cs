namespace TransportApi.Models;

public class TransportOrder : IReservable
{
    public int Id { get; set; }

    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public int DriverId { get; set; }
    public Driver? Driver { get; set; }

    public string CargoDescription { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; }

    public void AssignDriver(Driver driver)
    {
        Driver = driver;
        DriverId = driver.Id;
    }

    public void Start()
    {
        StartTime = DateTime.UtcNow;
        if (Vehicle != null) Vehicle.IsAvailable = false;
        if (Driver != null) Driver.IsAvailable = false;
    }

    public void Complete()
    {
        IsCompleted = true;
        EndTime = DateTime.UtcNow;
        if (Vehicle != null) Vehicle.IsAvailable = true;
        if (Driver != null) Driver.IsAvailable = true;
    }
}