using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.CreateShipment;

/// <summary>Command: staff creates a shipment for a packed order → moves it to Shipping (UC-22).</summary>
public record CreateShipmentCommand(Guid OrderId, string Provider, string? TrackingNumber) : IRequest<OrderDto>;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, OrderDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateShipmentCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        if (request.Provider is not ("GHTK" or "GHN"))
            throw new ConflictException("Nhà vận chuyển phải là GHTK hoặc GHN.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        if (!OrderStatus.CanTransition(order.Status, OrderStatus.Shipping))
            throw new ConflictException($"Đơn phải ở trạng thái '{OrderStatus.Packed}' mới tạo được shipment (hiện tại: '{order.Status}').");

        var now = DateTime.UtcNow;

        // A real integration would call the GHTK/GHN API here to get a tracking number.
        _db.Shipments.Add(new Shipment
        {
            OrderId = order.Id,
            Provider = request.Provider,
            TrackingNumber = request.TrackingNumber ?? $"{request.Provider}{now:yyyyMMddHHmmss}",
            Status = "Shipping",
            ShippedAt = now
        });

        order.Status = OrderStatus.Shipping;
        order.ShippedAt = now;

        _db.OrderAuditLogs.Add(new OrderAuditLog
        {
            OrderId = order.Id,
            FromStatus = OrderStatus.Packed,
            ToStatus = OrderStatus.Shipping,
            ChangedBy = _currentUser.UserId,
            Reason = $"Shipment created ({request.Provider})",
            ChangedAt = now
        });

        await _db.SaveChangesAsync(cancellationToken);
        return await OrderMapper.BuildDtoAsync(_db, order.Id, cancellationToken);
    }
}
