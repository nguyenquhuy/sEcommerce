using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Coupons.CreateCoupon;

/// <summary>Command: create a coupon (UC-32). Returns the new Id.</summary>
public record CreateCouponCommand(
    string Code,
    string Type,
    decimal Value,
    decimal MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? MaxUsage,
    int MaxUsagePerUser,
    DateTime StartAt,
    DateTime EndAt)
    : IRequest<Guid>;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _db.Coupons.AnyAsync(c => c.Code == code, cancellationToken))
            throw new ConflictException($"Mã '{code}' đã tồn tại.");

        var coupon = new Coupon
        {
            Code = code,
            Type = request.Type,
            Value = request.Value,
            MinOrderAmount = request.MinOrderAmount,
            MaxDiscountAmount = request.MaxDiscountAmount,
            MaxUsage = request.MaxUsage,
            MaxUsagePerUser = request.MaxUsagePerUser,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            IsActive = true
        };

        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync(cancellationToken);
        return coupon.Id;
    }
}
