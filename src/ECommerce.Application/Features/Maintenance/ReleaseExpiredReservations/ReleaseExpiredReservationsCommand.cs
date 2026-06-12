using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Maintenance.ReleaseExpiredReservations;

/// <summary>Command (background, BR-03): release stock reserved by carts whose checkout window has lapsed. Returns # carts released.</summary>
public record ReleaseExpiredReservationsCommand : IRequest<int>;

public class ReleaseExpiredReservationsCommandHandler : IRequestHandler<ReleaseExpiredReservationsCommand, int>
{
    private readonly IAppDbContext _db;

    public ReleaseExpiredReservationsCommandHandler(IAppDbContext db) => _db = db;

    public async Task<int> Handle(ReleaseExpiredReservationsCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var expiredCarts = await _db.Carts
            .Where(c => c.ReservedUntil != null && c.ReservedUntil < now)
            .Include(c => c.Items).ThenInclude(i => i.Variant).ThenInclude(v => v.Inventory)
            .ToListAsync(cancellationToken);

        var released = 0;
        foreach (var cart in expiredCarts)
        {
            var changed = false;
            foreach (var item in cart.Items)
            {
                if (item.ReservedQuantity > 0 && item.Variant.Inventory is { } inv)
                {
                    inv.Reserved -= item.ReservedQuantity;
                    item.ReservedQuantity = 0;
                    changed = true;
                }
            }
            cart.ReservedUntil = null;
            if (changed) released++;
        }

        if (expiredCarts.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return released;
    }
}
