namespace ECommerce.Domain.Enums;

/// <summary>Order status values and the allowed transitions (§9.1 state machine).</summary>
public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Packed = "Packed";
    public const string Shipping = "Shipping";
    public const string Delivered = "Delivered";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string Returning = "Returning";
    public const string Returned = "Returned";
    public const string Refunded = "Refunded";

    private static readonly Dictionary<string, string[]> Allowed = new()
    {
        [Pending] = new[] { Confirmed, Cancelled },
        [Confirmed] = new[] { Packed, Cancelled },
        [Packed] = new[] { Shipping },
        [Shipping] = new[] { Delivered },
        [Delivered] = new[] { Completed, Returning },
        [Returning] = new[] { Returned },
        [Returned] = new[] { Refunded },
        [Completed] = Array.Empty<string>(),
        [Cancelled] = Array.Empty<string>(),
        [Refunded] = Array.Empty<string>()
    };

    public static bool CanTransition(string from, string to)
        => Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
}
