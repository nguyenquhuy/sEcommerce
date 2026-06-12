using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.AddProductVariant;

/// <summary>Command: add a sellable variant to a product and create its inventory row. Returns the new variant Id.</summary>
public record AddProductVariantCommand(
    Guid ProductId,
    string Sku,
    decimal Price,
    string? AttributesJson = null,
    decimal? ComparePrice = null,
    string? ImageUrl = null,
    decimal? Weight = null,
    int InitialOnHand = 0,
    bool IsActive = true)
    : IRequest<Guid>;

public class AddProductVariantCommandHandler : IRequestHandler<AddProductVariantCommand, Guid>
{
    private readonly IAppDbContext _db;

    public AddProductVariantCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(AddProductVariantCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);
        if (!productExists)
            throw new NotFoundException(nameof(Product), request.ProductId);

        var skuTaken = await _db.ProductVariants.AnyAsync(v => v.Sku == request.Sku, cancellationToken);
        if (skuTaken)
            throw new ConflictException($"SKU '{request.Sku}' đã tồn tại.");

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            Sku = request.Sku,
            Price = request.Price,
            AttributesJson = request.AttributesJson,
            ComparePrice = request.ComparePrice,
            ImageUrl = request.ImageUrl,
            Weight = request.Weight,
            IsActive = request.IsActive,
            Inventory = new Inventory
            {
                OnHand = request.InitialOnHand,
                Reserved = 0
            }
        };

        _db.ProductVariants.Add(variant);
        await _db.SaveChangesAsync(cancellationToken);

        return variant.Id;
    }
}
