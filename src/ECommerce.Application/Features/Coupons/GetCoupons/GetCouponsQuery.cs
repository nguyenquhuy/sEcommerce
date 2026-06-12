using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Coupons.GetCoupons;

/// <summary>Query: all coupons for admin management (UC-32).</summary>
public record GetCouponsQuery : IRequest<IReadOnlyList<CouponDto>>;

public class GetCouponsQueryHandler : IRequestHandler<GetCouponsQuery, IReadOnlyList<CouponDto>>
{
    private readonly IAppDbContext _db;

    public GetCouponsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CouponDto>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
        => await _db.Coupons
            .AsNoTracking()
            .OrderByDescending(c => c.StartAt)
            .Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                Type = c.Type,
                Value = c.Value,
                MinOrderAmount = c.MinOrderAmount,
                MaxDiscountAmount = c.MaxDiscountAmount,
                MaxUsage = c.MaxUsage,
                MaxUsagePerUser = c.MaxUsagePerUser,
                UsedCount = c.UsedCount,
                StartAt = c.StartAt,
                EndAt = c.EndAt,
                IsActive = c.IsActive
            })
            .ToListAsync(cancellationToken);
}
