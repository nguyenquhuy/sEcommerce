using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.UpdateProductVariant;

/// <summary>Command: update a variant's selling attributes (not its stock — use AdjustInventory for that).</summary>
public record UpdateProductVariantCommand(
    Guid Id,
    string Sku,
    decimal Price,
    string? AttributesJson,
    decimal? ComparePrice,
    string? ImageUrl,
    decimal? Weight,
    bool IsActive)
    : IRequest;

public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand>
{
    private readonly IAppDbContext _db;

    public UpdateProductVariantCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductVariant), request.Id);

        var skuTaken = await _db.ProductVariants
            .AnyAsync(v => v.Sku == request.Sku && v.Id != request.Id, cancellationToken);
        if (skuTaken)
            throw new ConflictException($"SKU '{request.Sku}' đã tồn tại.");

        variant.Sku = request.Sku;
        variant.Price = request.Price;
        variant.AttributesJson = request.AttributesJson;
        variant.ComparePrice = request.ComparePrice;
        variant.ImageUrl = request.ImageUrl;
        variant.Weight = request.Weight;
        variant.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
