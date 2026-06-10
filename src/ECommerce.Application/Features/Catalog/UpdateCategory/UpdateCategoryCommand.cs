using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.UpdateCategory;

/// <summary>Command: update an existing category.</summary>
public record UpdateCategoryCommand(Guid Id, string Name, string Slug, Guid? ParentId, int SortOrder, bool IsActive)
    : IRequest;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IAppDbContext _db;

    public UpdateCategoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
