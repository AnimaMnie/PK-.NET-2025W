namespace TransportApi.Models;

public class Van : Vehicle
{
    public double CargoVolume { get; set; } // m3

    public override string GetInfo()
        => $"Van {RegistrationNumber} ({Model}), cargo volume: {CargoVolume} m3";
}