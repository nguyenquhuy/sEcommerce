namespace ECommerce.Application.Features.Cart;

/// <summary>The full cart view: lines + computed money. Prices are CURRENT (not snapshots).</summary>
public class CartDto
{
    public Guid Id { get; set; }
    public IReadOnlyList<CartItemDto> Items { get; set; } = Array.Empty<CartItemDto>();
    public int TotalQuantity { get; set; }
    public decimal Subtotal { get; set; }
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; }
    /// <summary>Subtotal − Discount. Shipping is added later at checkout.</summary>
    public decimal Total { get; set; }
}

/// <summary>A single cart line with the variant's current price and live availability.</summary>
public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSlug { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? AttributesJson { get; set; }
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public int Available { get; set; }
    /// <summary>True when requested quantity exceeds current availability.</summary>
    public bool IsOutOfStock { get; set; }
}
