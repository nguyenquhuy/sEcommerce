using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.CreateProduct;

/// <summary>Command: create a product (UC-30). Returns the new Id.</summary>
public record CreateProductCommand(
    string Name,
    string Slug,
    Guid CategoryId,
    decimal BasePrice,
    string? Description = null,
    string? SeoTitle = null,
    string? SeoDescription = null,
    bool IsActive = true)
    : IRequest<Guid>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var slugTaken = await _db.Products.AnyAsync(p => p.Slug == request.Slug, cancellationToken);
        if (slugTaken)
            throw new ConflictException($"Slug '{request.Slug}' đã được sử dụng.");

        var product = new Product
        {
            Name = request.Name,
            Slug = request.Slug,
            CategoryId = request.CategoryId,
            BasePrice = request.BasePrice,
            Description = request.Description,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            IsActive = request.IsActive,
            IsDeleted = false
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
