using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.GetProducts;

/// <summary>Query: paged product catalog (UC-01) with optional category/keyword filter and sort.</summary>
public record GetProductsQuery(
    string? Keyword = null,
    Guid? CategoryId = null,
    string? Sort = null, // newest | price_asc | price_desc | name
    int Page = 1,
    int PageSize = 20,
    bool IncludeInactive = false) // admin listing
    : IRequest<PagedResult<ProductListItemDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IAppDbContext _db;

    public GetProductsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = _db.Products.AsNoTracking().Where(p => !p.IsDeleted);

        if (!request.IncludeInactive)
            query = query.Where(p => p.IsActive);

        if (request.CategoryId is { } categoryId)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(p => p.Name.Contains(keyword));
        }

        query = request.Sort switch
        {
            "price_asc" => query.OrderBy(p => p.BasePrice),
            "price_desc" => query.OrderByDescending(p => p.BasePrice),
            "name" => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.CreatedAt) // newest (default)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Slug = p.Slug,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                BasePrice = p.BasePrice,
                IsActive = p.IsActive,
                FromPrice = p.Variants.Where(v => v.IsActive).Select(v => (decimal?)v.Price).Min() ?? p.BasePrice,
                ThumbnailUrl = p.Variants
                    .Where(v => v.IsActive && v.ImageUrl != null)
                    .OrderBy(v => v.Price)
                    .Select(v => v.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductListItemDto>(items, page, pageSize, totalCount);
    }
}
