using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.GetCategoryById;

/// <summary>Query: get a single category by id (null if not found).</summary>
public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IAppDbContext _db;

    public GetCategoryByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Name = c.Name,
                Slug = c.Slug,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
