namespace StrategyMethod.Models;

public class Order
{
    public decimal Weight { get; set; }
    public decimal Value { get; set; }

    public override string ToString()
    {
        return $"Zamówienie [Waga={Weight} kg, Wartość={Value} zł]";
    }
}


