using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Coupons.UpdateCoupon;

/// <summary>Command: update a coupon's rules / activation (UC-32). Code is immutable.</summary>
public record UpdateCouponCommand(
    Guid Id,
    decimal Value,
    decimal MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? MaxUsage,
    int MaxUsagePerUser,
    DateTime StartAt,
    DateTime EndAt,
    bool IsActive)
    : IRequest;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand>
{
    private readonly IAppDbContext _db;

    public UpdateCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Coupon), request.Id);

        coupon.Value = request.Value;
        coupon.MinOrderAmount = request.MinOrderAmount;
        coupon.MaxDiscountAmount = request.MaxDiscountAmount;
        coupon.MaxUsage = request.MaxUsage;
        coupon.MaxUsagePerUser = request.MaxUsagePerUser;
        coupon.StartAt = request.StartAt;
        coupon.EndAt = request.EndAt;
        coupon.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
