using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.RemoveCoupon;

/// <summary>Command: clear the coupon from the cart.</summary>
public record RemoveCouponCommand(string? SessionId) : IRequest<CartDto>;

public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveCouponCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(RemoveCouponCommand request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var cart = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");

        cart.CouponId = null;
        await _db.SaveChangesAsync(cancellationToken);

        return await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
    }
}
