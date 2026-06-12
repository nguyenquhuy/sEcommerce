using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Maintenance.AutoCompleteOrders;

/// <summary>Command (background, BR-19): auto-complete orders delivered &gt; 3 days ago with no action. Returns # completed.</summary>
public record AutoCompleteOrdersCommand : IRequest<int>;

public class AutoCompleteOrdersCommandHandler : IRequestHandler<AutoCompleteOrdersCommand, int>
{
    private const int AutoCompleteAfterDays = 3;
    private readonly IAppDbContext _db;

    public AutoCompleteOrdersCommandHandler(IAppDbContext db) => _db = db;

    public async Task<int> Handle(AutoCompleteOrdersCommand request, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-AutoCompleteAfterDays);

        var orders = await _db.Orders
            .Where(o => o.Status == OrderStatus.Delivered && o.DeliveredAt != null && o.DeliveredAt < cutoff)
            .ToListAsync(cancellationToken);

        foreach (var order in orders)
        {
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
            _db.OrderAuditLogs.Add(new OrderAuditLog
            {
                OrderId = order.Id,
                FromStatus = OrderStatus.Delivered,
                ToStatus = OrderStatus.Completed,
                ChangedBy = null, // system
                Reason = "Auto-completed after delivery window",
                ChangedAt = DateTime.UtcNow
            });
        }

        if (orders.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return orders.Count;
    }
}
