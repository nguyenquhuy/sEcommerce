using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Returns.ProcessReturn;

/// <summary>Command: staff handles a return — approve / reject / receive / refund (UC-23, §8.5).</summary>
public record ProcessReturnCommand(Guid Id, string Action, string? StaffNote) : IRequest<ReturnRequestDto>;

public class ProcessReturnCommandHandler : IRequestHandler<ProcessReturnCommand, ReturnRequestDto>
{
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string Receive = "Receive";
    public const string Refund = "Refund";

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ProcessReturnCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReturnRequestDto> Handle(ProcessReturnCommand request, CancellationToken cancellationToken)
    {
        var ret = await _db.ReturnRequests
            .Include(r => r.Items).ThenInclude(i => i.OrderItem)
            .Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ReturnRequest), request.Id);

        ret.StaffNote = request.StaffNote ?? ret.StaffNote;
        var now = DateTime.UtcNow;

        switch (request.Action)
        {
            case Approve:
                Require(ret.Status == ReturnStatus.Pending, "Chỉ duyệt được yêu cầu đang chờ.");
                ret.Status = ReturnStatus.Approved;
                break;

            case Reject:
                Require(ret.Status == ReturnStatus.Pending, "Chỉ từ chối được yêu cầu đang chờ.");
                ret.Status = ReturnStatus.Rejected;
                RevertOrderToDelivered(ret.Order, now);
                break;

            case Receive:
                Require(ret.Status == ReturnStatus.Approved, "Chỉ nhận hàng khi đã duyệt.");
                ret.Status = ReturnStatus.Received;
                ret.Order.Status = OrderStatus.Returned; // Returning → Returned
                Audit(ret.OrderId, OrderStatus.Returning, OrderStatus.Returned, "Return received", now);
                break;

            case Refund:
                Require(ret.Status == ReturnStatus.Received, "Chỉ hoàn tiền khi đã nhận hàng.");
                await RestoreStockAsync(ret, cancellationToken);
                ret.Status = ReturnStatus.Refunded;
                ret.Order.Status = OrderStatus.Refunded; // Returned → Refunded
                Audit(ret.OrderId, OrderStatus.Returned, OrderStatus.Refunded, "Refund processed", now);
                break;

            default:
                throw new ConflictException($"Action '{request.Action}' không hợp lệ (Approve/Reject/Receive/Refund).");
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await ReturnMapper.BuildDtoAsync(_db, ret.Id, cancellationToken);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new ConflictException(message);
    }

    private void RevertOrderToDelivered(Order order, DateTime now)
    {
        var from = order.Status;
        order.Status = OrderStatus.Delivered;
        Audit(order.Id, from, OrderStatus.Delivered, "Return rejected", now);
    }

    private async Task RestoreStockAsync(ReturnRequest ret, CancellationToken ct)
    {
        var variantIds = ret.Items.Select(i => i.OrderItem.VariantId).Distinct().ToList();
        var inventories = await _db.Inventories
            .Where(inv => variantIds.Contains(inv.VariantId))
            .ToDictionaryAsync(inv => inv.VariantId, ct);

        foreach (var item in ret.Items)
            if (inventories.TryGetValue(item.OrderItem.VariantId, out var inv))
                inv.OnHand += item.Quantity;
    }

    private void Audit(Guid orderId, string from, string to, string reason, DateTime at)
        => _db.OrderAuditLogs.Add(new OrderAuditLog
        {
            OrderId = orderId,
            FromStatus = from,
            ToStatus = to,
            ChangedBy = _currentUser.UserId,
            Reason = reason,
            ChangedAt = at
        });
}
