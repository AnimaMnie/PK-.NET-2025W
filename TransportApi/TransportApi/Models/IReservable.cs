namespace TransportApi.Models;

public interface IReservable
{
    void AssignDriver(Driver driver);
    void Start();
    void Complete();
}