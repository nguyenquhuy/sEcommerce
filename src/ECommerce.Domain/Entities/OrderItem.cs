using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Order line with price/name snapshot (BR-04/05). Maps to ORDER_ITEMS.</summary>
public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public string VariantSkuSnapshot { get; set; } = string.Empty;
    public string? VariantAttributesSnapshot { get; set; }
    public decimal UnitPriceSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
