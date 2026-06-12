using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.AdminListOrders;

/// <summary>Query: all orders for the back office, filter by status / order-number search (UC-20).</summary>
public record AdminListOrdersQuery(string? Status = null, string? Keyword = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<OrderListItemDto>>;

public class AdminListOrdersQueryHandler : IRequestHandler<AdminListOrdersQuery, PagedResult<OrderListItemDto>>
{
    private readonly IAppDbContext _db;

    public AdminListOrdersQueryHandler(IAppDbContext db) => _db = db;

    public async Task<PagedResult<OrderListItemDto>> Handle(AdminListOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = _db.Orders.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(o => o.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kw = request.Keyword.Trim();
            query = query.Where(o => o.OrderNumber.Contains(kw));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListItemDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                Total = o.Total,
                PaymentMethod = o.PaymentMethod,
                ItemCount = o.Items.Count,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderListItemDto>(items, page, pageSize, total);
    }
}
