using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.DeleteProductVariant;

/// <summary>Command: delete a variant (and its inventory row, via cascade). Blocked if it was ever ordered.</summary>
public record DeleteProductVariantCommand(Guid Id) : IRequest;

public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
{
    private readonly IAppDbContext _db;

    public DeleteProductVariantCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductVariant), request.Id);

        // A variant referenced by past orders must not be hard-deleted (snapshot integrity, BR-04/05).
        var hasOrders = await _db.OrderItems.AnyAsync(i => i.VariantId == request.Id, cancellationToken);
        if (hasOrders)
            throw new ConflictException("Không thể xóa variant đã từng được đặt hàng. Hãy đặt IsActive = false thay vì xóa.");

        _db.ProductVariants.Remove(variant);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
