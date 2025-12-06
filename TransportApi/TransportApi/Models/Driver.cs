namespace TransportApi.Models;

public class Driver
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";
}