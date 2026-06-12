using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Pricing;
using ECommerce.Application.Features.Cart;
using ECommerce.Application.Features.Checkout;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Checkout.StartCheckout;

/// <summary>
/// Command: enter checkout — reserve stock for 15 minutes (BR-02) and return the summary
/// with shipping options. Re-runnable: it releases the prior reservation first.
/// </summary>
public record StartCheckoutCommand : IRequest<CheckoutSummaryDto>;

public class StartCheckoutCommandHandler : IRequestHandler<StartCheckoutCommand, CheckoutSummaryDto>
{
    private static readonly TimeSpan ReservationWindow = TimeSpan.FromMinutes(15);

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StartCheckoutCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CheckoutSummaryDto> Handle(StartCheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Cần đăng nhập để thanh toán.");

        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Variant).ThenInclude(v => v.Inventory)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
            throw new ConflictException("Giỏ hàng trống.");

        // Release any reservation this cart already holds, so re-entry doesn't double-count.
        foreach (var item in cart.Items)
        {
            if (item.ReservedQuantity > 0 && item.Variant.Inventory is { } inv0)
                inv0.Reserved -= item.ReservedQuantity;
            item.ReservedQuantity = 0;
        }

        // Validate availability and reserve fresh.
        foreach (var item in cart.Items)
        {
            if (!item.Variant.IsActive)
                throw new ConflictException($"Sản phẩm {item.Variant.Sku} hiện không bán.");

            var inv = item.Variant.Inventory
                ?? throw new ConflictException($"Sản phẩm {item.Variant.Sku} chưa có tồn kho.");

            if (item.Quantity > inv.Available) // BR-01
                throw new ConflictException($"Sản phẩm {item.Variant.Sku} chỉ còn {inv.Available}.");

            inv.Reserved += item.Quantity;
            item.ReservedQuantity = item.Quantity;
        }

        cart.ReservedUntil = DateTime.UtcNow.Add(ReservationWindow);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
        var afterDiscount = dto.Subtotal - dto.Discount;

        return new CheckoutSummaryDto
        {
            CartId = cart.Id,
            Items = dto.Items,
            Subtotal = dto.Subtotal,
            CouponCode = dto.CouponCode,
            Discount = dto.Discount,
            ReservedUntil = cart.ReservedUntil.Value,
            ShippingOptions = new[]
            {
                new ShippingOptionDto(ShippingCalculator.Standard, ShippingCalculator.Calculate(ShippingCalculator.Standard, afterDiscount)),
                new ShippingOptionDto(ShippingCalculator.Express, ShippingCalculator.Calculate(ShippingCalculator.Express, afterDiscount))
            }
        };
    }
}
