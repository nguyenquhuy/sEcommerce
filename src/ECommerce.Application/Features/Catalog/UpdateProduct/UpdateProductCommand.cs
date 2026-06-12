using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.UpdateProduct;

/// <summary>Command: update an existing product (UC-30).</summary>
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Slug,
    Guid CategoryId,
    decimal BasePrice,
    string? Description,
    string? SeoTitle,
    string? SeoDescription,
    bool IsActive)
    : IRequest;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IAppDbContext _db;

    public UpdateProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var slugTaken = await _db.Products
            .AnyAsync(p => p.Slug == request.Slug && p.Id != request.Id, cancellationToken);
        if (slugTaken)
            throw new ConflictException($"Slug '{request.Slug}' đã được sử dụng.");

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.CategoryId = request.CategoryId;
        product.BasePrice = request.BasePrice;
        product.Description = request.Description;
        product.SeoTitle = request.SeoTitle;
        product.SeoDescription = request.SeoDescription;
        product.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
