using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>A buyable variant of a product (own SKU, price, stock). Maps to PRODUCT_VARIANTS.</summary>
public class ProductVariant : AuditableEntity
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? AttributesJson { get; set; }
    public decimal Price { get; set; }
    public decimal? ComparePrice { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Weight { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Product Product { get; set; } = null!;
    public Inventory? Inventory { get; set; }
}
