using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>A sellable product (logical). Real stock/price lives on its variants. Maps to PRODUCTS.</summary>
public class Product : AuditableEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
