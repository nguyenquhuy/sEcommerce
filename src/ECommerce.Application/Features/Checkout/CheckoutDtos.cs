using ECommerce.Application.Features.Cart;

namespace ECommerce.Application.Features.Checkout;

/// <summary>Returned by StartCheckout: the reserved cart snapshot + shipping options (§8.2 step 1-2).</summary>
public class CheckoutSummaryDto
{
    public Guid CartId { get; set; }
    public IReadOnlyList<CartItemDto> Items { get; set; } = Array.Empty<CartItemDto>();
    public decimal Subtotal { get; set; }
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; }
    public DateTime ReservedUntil { get; set; }
    public IReadOnlyList<ShippingOptionDto> ShippingOptions { get; set; } = Array.Empty<ShippingOptionDto>();
}

public record ShippingOptionDto(string Method, decimal Fee);
