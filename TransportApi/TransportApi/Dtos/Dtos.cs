namespace TransportApi.Models;

public record VehicleCreateDto(
    string Type,
    string RegistrationNumber,
    string Model,
    double? TrailerLength,
    double? CargoVolume
);

public record DriverCreateDto(
    string FirstName,
    string LastName
);

public record OrderCreateDto(
    int VehicleId,
    int DriverId,
    string CargoDescription
);