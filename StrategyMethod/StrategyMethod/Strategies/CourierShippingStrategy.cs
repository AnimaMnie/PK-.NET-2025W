using StrategyMethod.Models;

namespace StrategyMethod.Strategies;
public class CourierShippingStrategy : IShippingStrategy
{
    public string Name => "Kurier";
    public decimal CalculateCost(Order order)
    {
        return 15.00m + order.Value * 0.01m;
    }
}


