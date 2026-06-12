using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Returns.RequestReturn;

/// <summary>Command: customer requests a return for delivered items (UC-11, BR-10). Returns the request Id.</summary>
public record RequestReturnCommand(Guid OrderId, string Reason, IReadOnlyList<ReturnItemInput> Items)
    : IRequest<ReturnRequestDto>;

public class RequestReturnCommandHandler : IRequestHandler<RequestReturnCommand, ReturnRequestDto>
{
    private const int ReturnWindowDays = 7; // BR-10
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RequestReturnCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReturnRequestDto> Handle(RequestReturnCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        if (order.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền trả đơn hàng này.");

        if (order.Status != OrderStatus.Delivered)
            throw new ConflictException("Chỉ trả hàng được khi đơn ở trạng thái 'Delivered'.");

        // BR-10: within 7 days of delivery.
        if (order.DeliveredAt is null || order.DeliveredAt.Value.AddDays(ReturnWindowDays) < DateTime.UtcNow)
            throw new ConflictException($"Đã quá thời hạn trả hàng ({ReturnWindowDays} ngày).");

        if (request.Items.Count == 0)
            throw new ConflictException("Chọn ít nhất 1 sản phẩm để trả.");

        var returnReq = new ReturnRequest
        {
            OrderId = order.Id,
            UserId = userId,
            Status = ReturnStatus.Pending,
            Reason = request.Reason
        };

        decimal refund = 0m;
        foreach (var input in request.Items)
        {
            var line = order.Items.FirstOrDefault(i => i.Id == input.OrderItemId)
                ?? throw new ConflictException($"Sản phẩm {input.OrderItemId} không thuộc đơn hàng.");

            if (input.Quantity < 1 || input.Quantity > line.Quantity)
                throw new ConflictException($"Số lượng trả không hợp lệ cho sản phẩm {line.ProductNameSnapshot}.");

            refund += line.UnitPriceSnapshot * input.Quantity;
            returnReq.Items.Add(new ReturnItem { OrderItemId = line.Id, Quantity = input.Quantity });
        }
        returnReq.RefundAmount = refund;

        order.Status = OrderStatus.Returning; // Delivered → Returning
        _db.OrderAuditLogs.Add(new OrderAuditLog
        {
            OrderId = order.Id,
            FromStatus = OrderStatus.Delivered,
            ToStatus = OrderStatus.Returning,
            ChangedBy = userId,
            Reason = "Return requested",
            ChangedAt = DateTime.UtcNow
        });

        _db.ReturnRequests.Add(returnReq);
        await _db.SaveChangesAsync(cancellationToken);

        return await ReturnMapper.BuildDtoAsync(_db, returnReq.Id, cancellationToken);
    }
}
