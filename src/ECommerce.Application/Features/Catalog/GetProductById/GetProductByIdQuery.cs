using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.GetProductById;

/// <summary>Query: product detail by id (admin — includes inactive, excludes hard-deleted-by-flag). Null if not found.</summary>
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly IAppDbContext _db;

    public GetProductByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id && !p.IsDeleted)
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
