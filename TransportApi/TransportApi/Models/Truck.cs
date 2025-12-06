namespace TransportApi.Models;

public class Truck : Vehicle
{
    public double TrailerLength { get; set; } // w metrach

    public override string GetInfo()
        => $"Truck {RegistrationNumber} ({Model}), trailer: {TrailerLength} m";
}