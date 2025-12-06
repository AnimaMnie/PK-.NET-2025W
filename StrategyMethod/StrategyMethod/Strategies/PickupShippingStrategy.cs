using StrategyMethod.Models;

namespace StrategyMethod.Strategies;
public class PickupShippingStrategy : IShippingStrategy
{
    public string Name => "Odbiór osobisty";
    public decimal CalculateCost(Order order)
    {
        return 0m;
    }
}