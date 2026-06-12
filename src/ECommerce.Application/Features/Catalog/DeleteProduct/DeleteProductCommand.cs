using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.DeleteProduct;

/// <summary>Command: soft-delete a product (BR-16). Old orders keep referencing it via snapshots.</summary>
public record DeleteProductCommand(Guid Id) : IRequest;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IAppDbContext _db;

    public DeleteProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        // BR-16: soft delete only — also hide it from the storefront.
        product.IsDeleted = true;
        product.IsActive = false;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
