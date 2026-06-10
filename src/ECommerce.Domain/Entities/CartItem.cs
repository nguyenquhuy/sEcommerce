using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Line in a cart. Maps to CART_ITEMS.</summary>
public class CartItem : AuditableEntity
{
    public Guid CartId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }

    // Navigation
    public Cart Cart { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
