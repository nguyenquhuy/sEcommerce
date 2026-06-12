using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Pricing;
using ECommerce.Application.Features.Cart;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.ApplyCoupon;

/// <summary>Command: apply a coupon code to the cart (UC-04). Validates it against the current subtotal (BR-20).</summary>
public record ApplyCouponCommand(string Code, string? SessionId) : IRequest<CartDto>;

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ApplyCouponCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var cart = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .Include(c => c.Items).ThenInclude(i => i.Variant)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");

        if (cart.Items.Count == 0)
            throw new ConflictException("Giỏ hàng trống, không thể áp mã.");

        var code = request.Code.Trim().ToUpperInvariant();
        var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code, cancellationToken)
            ?? throw new NotFoundException($"Mã giảm giá '{code}' không tồn tại.");

        var subtotal = cart.Items.Sum(i => i.Variant.Price * i.Quantity);
        CouponCalculator.EnsureApplicable(coupon, subtotal, DateTime.UtcNow); // throws if invalid

        cart.CouponId = coupon.Id;
        await _db.SaveChangesAsync(cancellationToken);

        return await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
    }
}
