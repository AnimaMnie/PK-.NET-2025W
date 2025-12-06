namespace TransportApi.Models;

public abstract class Vehicle
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public abstract string GetInfo();
}