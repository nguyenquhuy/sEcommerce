using System.Text.Json;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Pricing;
using ECommerce.Application.Features.Orders;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Checkout.ConfirmOrder;

/// <summary>Command: place the order (§8.2 step 4). Snapshots prices/names, decrements stock, all in one transaction.</summary>
public record ConfirmOrderCommand(
    ShippingAddressDto ShippingAddress,
    string ShippingMethod,
    string PaymentMethod,
    string? Note)
    : IRequest<OrderDto>;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderDto>
{
    private const decimal CodMinOrderAmount = 50_000m; // BR-06
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmailService _email;

    public ConfirmOrderCommandHandler(IAppDbContext db, ICurrentUserService currentUser, IEmailService email)
    {
        _db = db;
        _currentUser = currentUser;
        _email = email;
    }

    public async Task<OrderDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Cần đăng nhập để đặt hàng.");

        if (!ShippingCalculator.IsValidMethod(request.ShippingMethod))
            throw new ConflictException("Phương thức vận chuyển không hợp lệ.");

        if (request.PaymentMethod is not (PaymentMethod.Cod or PaymentMethod.VnPay or PaymentMethod.MoMo))
            throw new ConflictException("Phương thức thanh toán không hợp lệ.");

        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Variant).ThenInclude(v => v.Inventory)
            .Include(c => c.Items).ThenInclude(i => i.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
            throw new ConflictException("Giỏ hàng trống.");

        var subtotal = cart.Items.Sum(i => i.Variant.Price * i.Quantity);

        // Coupon: re-validate (BR-20) + per-user usage limit (BR-07).
        decimal discount = 0m;
        Coupon? coupon = null;
        if (cart.CouponId is { } couponId)
        {
            coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == couponId, cancellationToken);
            if (coupon is not null)
            {
                CouponCalculator.EnsureApplicable(coupon, subtotal, DateTime.UtcNow);

                var usedByUser = await _db.Orders.CountAsync(
                    o => o.UserId == userId && o.CouponCode == coupon.Code && o.Status != OrderStatus.Cancelled,
                    cancellationToken);
                if (usedByUser >= coupon.MaxUsagePerUser)
                    throw new ConflictException($"Bạn đã dùng hết lượt cho mã '{coupon.Code}'.");

                discount = CouponCalculator.ComputeDiscount(coupon, subtotal);
            }
        }

        var afterDiscount = subtotal - discount;
        var shippingFee = ShippingCalculator.Calculate(request.ShippingMethod, afterDiscount);
        var total = afterDiscount + shippingFee;

        if (request.PaymentMethod == PaymentMethod.Cod && total < CodMinOrderAmount)
            throw new ConflictException($"Đơn COD tối thiểu {CodMinOrderAmount:N0}đ.");

        // BR-12: final stock re-check, then decrement on-hand and release our reservation.
        foreach (var item in cart.Items)
        {
            var inv = item.Variant.Inventory
                ?? throw new ConflictException($"Sản phẩm {item.Variant.Sku} chưa có tồn kho.");

            var effectiveAvailable = inv.OnHand - inv.Reserved + item.ReservedQuantity;
            if (item.Quantity > effectiveAvailable)
                throw new ConflictException($"Sản phẩm {item.Variant.Sku} không đủ tồn kho.");

            inv.OnHand -= item.Quantity;
            inv.Reserved -= item.ReservedQuantity;
        }

        var order = new Order
        {
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
            UserId = userId,
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            ShippingFee = shippingFee,
            Discount = discount,
            Total = total,
            CouponCode = coupon?.Code,
            ShippingAddressJson = JsonSerializer.Serialize(request.ShippingAddress),
            Note = request.Note,
            PaymentMethod = request.PaymentMethod
        };

        foreach (var item in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                VariantId = item.VariantId,
                ProductNameSnapshot = item.Variant.Product.Name,   // BR-05
                VariantSkuSnapshot = item.Variant.Sku,
                VariantAttributesSnapshot = item.Variant.AttributesJson,
                UnitPriceSnapshot = item.Variant.Price,            // BR-04
                Quantity = item.Quantity,
                LineTotal = item.Variant.Price * item.Quantity
            });
        }

        order.Payments.Add(new Domain.Entities.Payment
        {
            Method = request.PaymentMethod,
            Amount = total,
            Status = PaymentStatus.Pending
        });

        order.AuditLogs.Add(new OrderAuditLog
        {
            FromStatus = null,
            ToStatus = OrderStatus.Pending,
            ChangedBy = userId,
            Reason = "Order created",
            ChangedAt = DateTime.UtcNow
        });

        if (coupon is not null)
            coupon.UsedCount++;

        // Empty the cart and drop the reservation (it's now committed into the order).
        _db.CartItems.RemoveRange(cart.Items);
        cart.CouponId = null;
        cart.ReservedUntil = null;

        _db.Orders.Add(order);

        try
        {
            await _db.SaveChangesAsync(cancellationToken); // single transaction
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("Tồn kho vừa thay đổi, vui lòng thử lại.");
        }

        await _email.SendAsync(
            _currentUser.Email ?? "customer",
            $"Đơn hàng {order.OrderNumber} đã được tạo",
            $"Cảm ơn bạn đã đặt hàng. Tổng tiền: {total:N0}đ. Phương thức: {request.PaymentMethod}.",
            cancellationToken);

        return await OrderMapper.BuildDtoAsync(_db, order.Id, cancellationToken);
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken ct)
    {
        var prefix = $"ORD{DateTime.UtcNow:yyyyMMdd}-";
        var countToday = await _db.Orders.CountAsync(o => o.OrderNumber.StartsWith(prefix), ct);
        return $"{prefix}{countToday + 1:D4}";
    }
}
