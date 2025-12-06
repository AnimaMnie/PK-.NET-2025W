using StrategyMethod.Models;

namespace StrategyMethod.Strategies;
public class ParcelLockerShippingStrategy : IShippingStrategy
{
    public string Name => "Paczkomat";
    public decimal CalculateCost(Order order)
    {
        return 9.99m + order.Weight * 0.5m;
    }
}




