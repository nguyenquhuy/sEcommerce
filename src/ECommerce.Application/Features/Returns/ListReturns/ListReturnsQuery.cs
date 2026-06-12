using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Returns.ListReturns;

/// <summary>Query: return requests for the back office, optional status filter (UC-23).</summary>
public record ListReturnsQuery(string? Status = null) : IRequest<IReadOnlyList<ReturnRequestDto>>;

public class ListReturnsQueryHandler : IRequestHandler<ListReturnsQuery, IReadOnlyList<ReturnRequestDto>>
{
    private readonly IAppDbContext _db;

    public ListReturnsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ReturnRequestDto>> Handle(ListReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ReturnRequests.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(r => r.Status == request.Status);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReturnRequestDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                OrderNumber = r.Order.OrderNumber,
                Status = r.Status,
                Reason = r.Reason,
                StaffNote = r.StaffNote,
                RefundAmount = r.RefundAmount,
                CreatedAt = r.CreatedAt,
                Items = r.Items.Select(i => new ReturnItemDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductName = i.OrderItem.ProductNameSnapshot,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
