using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.CancelOrder;

/// <summary>Command: customer cancels their own order (UC-10, BR-09) and stock is restored.</summary>
public record CancelOrderCommand(Guid Id, string? Reason) : IRequest<OrderDto>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (order.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền hủy đơn hàng này.");

        // BR-09: only Pending or Confirmed, and not yet packed/shipped.
        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            throw new ConflictException($"Không thể hủy đơn ở trạng thái '{order.Status}'.");

        // Restore stock that was decremented at confirm.
        var variantIds = order.Items.Select(i => i.VariantId).ToList();
        var inventories = await _db.Inventories
            .Where(inv => variantIds.Contains(inv.VariantId))
            .ToDictionaryAsync(inv => inv.VariantId, cancellationToken);

        foreach (var item in order.Items)
            if (inventories.TryGetValue(item.VariantId, out var inv))
                inv.OnHand += item.Quantity;

        // Give back the coupon use.
        if (order.CouponCode is { } code)
        {
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
            if (coupon is not null && coupon.UsedCount > 0)
                coupon.UsedCount--;
        }

        var from = order.Status;
        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancelReason = request.Reason;

        _db.OrderAuditLogs.Add(new OrderAuditLog
        {
            OrderId = order.Id,
            FromStatus = from,
            ToStatus = OrderStatus.Cancelled,
            ChangedBy = userId,
            Reason = request.Reason ?? "Customer cancelled",
            ChangedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);
        return await OrderMapper.BuildDtoAsync(_db, order.Id, cancellationToken);
    }
}
