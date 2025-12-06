using StrategyMethod.Models;
using StrategyMethod.Strategies;

namespace StrategyMethod.Services;

public class ShippingCostCalculator
{
    private IShippingStrategy _strategy;
    public ShippingCostCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }
    public void SetStrategy(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }
    public decimal CalculateCost(Order order)
    {
        return _strategy.CalculateCost(order);
    }

    public string CurrentStrategyName => _strategy?.Name ?? "Brak strategii dostawy";
}