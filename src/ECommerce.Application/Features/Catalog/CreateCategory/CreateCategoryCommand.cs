using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Catalog.CreateCategory;

/// <summary>Command (also the input model): create a category, returns the new Id.</summary>
public record CreateCategoryCommand(string Name, string Slug, Guid? ParentId, int SortOrder = 0)
    : IRequest<Guid>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateCategoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Slug = request.Slug,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = true
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
