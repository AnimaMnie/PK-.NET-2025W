using StrategyMethod.Models;
using StrategyMethod.Services;
using StrategyMethod.Strategies;

namespace StrategyMethod;

public class Program
{
    public static void Main(string[] args)
    {
        var order = new Order
        {
            Weight = 2.5m,
            Value = 200m
        };
        Console.WriteLine(order);
        
        IShippingStrategy parcelLocker = new ParcelLockerShippingStrategy();
        var calculator = new ShippingCostCalculator(parcelLocker);
        ShowCost(calculator, order);
        
        IShippingStrategy courier = new CourierShippingStrategy();
        calculator.SetStrategy(courier);
        ShowCost(calculator, order);
        
        IShippingStrategy pickup = new PickupShippingStrategy();
        calculator.SetStrategy(pickup);
        ShowCost(calculator, order);
        
    }

    private static void ShowCost(ShippingCostCalculator calculator, Order order)
    {
        var cost = calculator.CalculateCost(order);
        Console.WriteLine($"Strategia: {calculator.CurrentStrategyName} -> koszt: {cost:0.00} zł");
    }
}