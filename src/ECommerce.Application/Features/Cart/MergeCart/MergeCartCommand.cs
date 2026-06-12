using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.MergeCart;

/// <summary>
/// Command: merge a guest cart (by session) into the logged-in user's cart after login
/// (§7.2 edge case). Quantities are summed and capped at availability; the guest cart is removed.
/// </summary>
public record MergeCartCommand(string SessionId) : IRequest<CartDto>;

public class MergeCartCommandHandler : IRequestHandler<MergeCartCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MergeCartCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(MergeCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Cần đăng nhập để gộp giỏ hàng.");

        var guestCart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == request.SessionId && c.UserId == null, cancellationToken);

        var memberCart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (memberCart is null)
        {
            memberCart = new Domain.Entities.Cart { UserId = userId, ExpiresAt = DateTime.UtcNow.AddDays(30) };
            _db.Carts.Add(memberCart);
        }

        if (guestCart is not null)
        {
            // Availability per variant to cap merged quantities (BR-01).
            var variantIds = guestCart.Items.Select(i => i.VariantId)
                .Concat(memberCart.Items.Select(i => i.VariantId)).Distinct().ToList();
            var available = await _db.Inventories
                .Where(inv => variantIds.Contains(inv.VariantId))
                .ToDictionaryAsync(inv => inv.VariantId, inv => inv.Available, cancellationToken);

            foreach (var gItem in guestCart.Items)
            {
                var target = memberCart.Items.FirstOrDefault(i => i.VariantId == gItem.VariantId);
                var cap = available.TryGetValue(gItem.VariantId, out var a) ? a : 0;

                if (target is null)
                    memberCart.Items.Add(new CartItem
                    {
                        VariantId = gItem.VariantId,
                        Quantity = Math.Min(gItem.Quantity, cap)
                    });
                else
                    target.Quantity = Math.Min(target.Quantity + gItem.Quantity, cap);
            }

            // Prefer the guest's coupon only if the member cart had none.
            if (memberCart.CouponId is null && guestCart.CouponId is not null)
                memberCart.CouponId = guestCart.CouponId;

            _db.Carts.Remove(guestCart);
        }

        memberCart.ExpiresAt = DateTime.UtcNow.AddDays(30);
        await _db.SaveChangesAsync(cancellationToken);

        return await CartHelper.BuildDtoAsync(_db, memberCart.Id, cancellationToken);
    }
}
