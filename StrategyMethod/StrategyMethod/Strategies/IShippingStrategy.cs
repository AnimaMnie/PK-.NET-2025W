using StrategyMethod.Models;

namespace StrategyMethod.Strategies;

public interface IShippingStrategy
{
    decimal CalculateCost(Order order);
    string Name { get; }
}


