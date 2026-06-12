namespace ECommerce.Application.Features.Catalog;

/// <summary>List/card read model for products (catalog grid).</summary>
public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
    /// <summary>Cheapest active variant price, falls back to BasePrice when no variant.</summary>
    public decimal? FromPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
}

/// <summary>Full read model for a product detail page (incl. variants + stock).</summary>
public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public IReadOnlyList<ProductVariantDto> Variants { get; set; } = Array.Empty<ProductVariantDto>();
}

/// <summary>Read model for a variant, including live availability from inventory.</summary>
public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? AttributesJson { get; set; }
    public decimal Price { get; set; }
    public decimal? ComparePrice { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Weight { get; set; }
    public bool IsActive { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available { get; set; }
}
