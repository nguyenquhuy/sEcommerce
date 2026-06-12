using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.GetProductBySlug;

/// <summary>Query: product detail by slug for the storefront (UC-02). Active, non-deleted only.</summary>
public record GetProductBySlugQuery(string Slug) : IRequest<ProductDetailDto?>;

public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, ProductDetailDto?>
{
    private readonly IAppDbContext _db;

    public GetProductBySlugQueryHandler(IAppDbContext db) => _db = db;

    public async Task<ProductDetailDto?> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.Slug == request.Slug && p.IsActive && !p.IsDeleted)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Slug = p.Slug,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                BasePrice = p.BasePrice,
                IsActive = p.IsActive,
                SeoTitle = p.SeoTitle,
                SeoDescription = p.SeoDescription,
                Variants = p.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.Price)
                    .Select(v => new ProductVariantDto
                    {
                        Id = v.Id,
                        Sku = v.Sku,
                        AttributesJson = v.AttributesJson,
                        Price = v.Price,
                        ComparePrice = v.ComparePrice,
                        ImageUrl = v.ImageUrl,
                        Weight = v.Weight,
                        IsActive = v.IsActive,
                        OnHand = v.Inventory != null ? v.Inventory.OnHand : 0,
                        Reserved = v.Inventory != null ? v.Inventory.Reserved : 0,
                        Available = v.Inventory != null ? v.Inventory.Available : 0
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
