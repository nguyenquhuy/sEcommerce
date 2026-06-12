using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reports.RevenueReport;

/// <summary>Query: revenue report between two dates — totals, by-day series and top products (UC-34).</summary>
public record RevenueReportQuery(DateTime From, DateTime To) : IRequest<RevenueReportDto>;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public IReadOnlyList<DailyRevenueDto> ByDay { get; set; } = Array.Empty<DailyRevenueDto>();
    public IReadOnlyList<TopProductDto> TopProducts { get; set; } = Array.Empty<TopProductDto>();
}

public record DailyRevenueDto(DateTime Date, decimal Revenue, int OrderCount);
public record TopProductDto(string ProductName, int QuantitySold, decimal Revenue);

public class RevenueReportQueryHandler : IRequestHandler<RevenueReportQuery, RevenueReportDto>
{
    // Statuses that count as realized revenue (paid/in-fulfilment), excluding Pending/Cancelled.
    // NOTE: List<string> (not string[]) so EF binds List.Contains → SQL IN cleanly on .NET 8.
    private static readonly List<string> CountedStatuses = new()
        { OrderStatus.Confirmed, OrderStatus.Packed, OrderStatus.Shipping, OrderStatus.Delivered, OrderStatus.Completed };

    private readonly IAppDbContext _db;

    public RevenueReportQueryHandler(IAppDbContext db) => _db = db;

    public async Task<RevenueReportDto> Handle(RevenueReportQuery request, CancellationToken cancellationToken)
    {
        var from = request.From.Date;
        var to = request.To.Date.AddDays(1); // inclusive end day

        var orders = _db.Orders.AsNoTracking()
            .Where(o => CountedStatuses.Contains(o.Status) && o.CreatedAt >= from && o.CreatedAt < to);

        var totalRevenue = await orders.SumAsync(o => (decimal?)o.Total, cancellationToken) ?? 0m;
        var orderCount = await orders.CountAsync(cancellationToken);

        // Project grouped aggregates to anonymous types in SQL, then map to positional-record DTOs in memory
        // (EF Core can't translate a record constructor inside a GroupBy projection).
        var byDayRaw = await orders
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.Total), Count = g.Count() })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        var topRaw = await _db.OrderItems.AsNoTracking()
            .Where(i => CountedStatuses.Contains(i.Order.Status) && i.Order.CreatedAt >= from && i.Order.CreatedAt < to)
            .GroupBy(i => i.ProductNameSnapshot)
            .Select(g => new { Name = g.Key, Quantity = g.Sum(i => i.Quantity), Revenue = g.Sum(i => i.LineTotal) })
            .OrderByDescending(p => p.Quantity)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new RevenueReportDto
        {
            TotalRevenue = totalRevenue,
            OrderCount = orderCount,
            ByDay = byDayRaw.Select(d => new DailyRevenueDto(d.Date, d.Revenue, d.Count)).ToList(),
            TopProducts = topRaw.Select(p => new TopProductDto(p.Name, p.Quantity, p.Revenue)).ToList()
        };
    }
}
