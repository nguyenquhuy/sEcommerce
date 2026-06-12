using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.ChangeOrderStatus;

/// <summary>
/// Command: staff/admin advances an order through the state machine (UC-21, §9.1).
/// Handles the forward happy path; Shipping is done via CreateShipment, cancel via CancelOrder.
/// </summary>
public record ChangeOrderStatusCommand(Guid Id, string ToStatus, string? Reason) : IRequest<OrderDto>;

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, OrderDto>
{
    private static readonly string[] AllowedTargets =
        { OrderStatus.Confirmed, OrderStatus.Packed, OrderStatus.Delivered, OrderStatus.Completed };

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ChangeOrderStatusCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedTargets.Contains(request.ToStatus))
            throw new ConflictException(
                $"Trạng thái '{request.ToStatus}' không hỗ trợ ở đây (Shipping dùng /shipment, hủy dùng /cancel).");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (!OrderStatus.CanTransition(order.Status, request.ToStatus))
            throw new ConflictException($"Không thể chuyển từ '{order.Status}' sang '{request.ToStatus}'.");

        var now = DateTime.UtcNow;
        var from = order.Status;
        order.Status = request.ToStatus;
        switch (request.ToStatus)
        {
            case OrderStatus.Confirmed: order.ConfirmedAt = now; break;
            case OrderStatus.Delivered: order.DeliveredAt = now; break;
            case OrderStatus.Completed: order.CompletedAt = now; break;
        }

        _db.OrderAuditLogs.Add(new OrderAuditLog
        {
            OrderId = order.Id,
            FromStatus = from,
            ToStatus = request.ToStatus,
            ChangedBy = _currentUser.UserId,
            Reason = request.Reason,
            ChangedAt = now
        });

        await _db.SaveChangesAsync(cancellationToken);
        return await OrderMapper.BuildDtoAsync(_db, order.Id, cancellationToken);
    }
}
