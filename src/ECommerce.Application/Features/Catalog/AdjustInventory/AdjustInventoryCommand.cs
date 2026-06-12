using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Catalog.AdjustInventory;

/// <summary>
/// Command: adjust on-hand stock for a variant (UC-24). <paramref name="Delta"/> can be
/// negative (stock-out) or positive (restock). Reserved is untouched. Returns new OnHand.
/// </summary>
public record AdjustInventoryCommand(Guid VariantId, int Delta, string? Reason = null) : IRequest<int>;

public class AdjustInventoryCommandHandler : IRequestHandler<AdjustInventoryCommand, int>
{
    private readonly IAppDbContext _db;

    public AdjustInventoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task<int> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _db.Inventories.FirstOrDefaultAsync(i => i.VariantId == request.VariantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Inventory), request.VariantId);

        var newOnHand = inventory.OnHand + request.Delta;
        if (newOnHand < 0)
            throw new ConflictException($"Tồn kho không thể âm. OnHand hiện tại = {inventory.OnHand}, delta = {request.Delta}.");

        // BR-01 guard: OnHand must stay >= Reserved so Available never goes negative.
        if (newOnHand < inventory.Reserved)
            throw new ConflictException($"OnHand ({newOnHand}) không được nhỏ hơn số đang giữ chỗ Reserved ({inventory.Reserved}).");

        inventory.OnHand = newOnHand;
        await _db.SaveChangesAsync(cancellationToken);

        return inventory.OnHand;
    }
}
