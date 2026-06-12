namespace ECommerce.Application.Common.Pricing;

/// <summary>
/// Simple shipping-fee rules (the doc's ShippingFeeCalculator). Flat fee per method with a
/// free-ship threshold. A real GHTK/GHN integration would replace this.
/// </summary>
public static class ShippingCalculator
{
    public const string Standard = "Standard";
    public const string Express = "Express";

    private const decimal FreeShipThreshold = 500_000m;

    public static bool IsValidMethod(string method) => method is Standard or Express;

    /// <summary>Fee for a method given the order subtotal (after discount). Standard is free over the threshold.</summary>
    public static decimal Calculate(string method, decimal subtotalAfterDiscount) => method switch
    {
        Standard => subtotalAfterDiscount >= FreeShipThreshold ? 0m : 30_000m,
        Express => 50_000m,
        _ => 30_000m
    };
}
