using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Coupons.DeleteCoupon;

/// <summary>Command: delete a coupon. Blocked if it has been used (kept for order history).</summary>
public record DeleteCouponCommand(Guid Id) : IRequest;

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand>
{
    private readonly IAppDbContext _db;

    public DeleteCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Coupon), request.Id);

        if (coupon.UsedCount > 0)
            throw new ConflictException("Mã đã được sử dụng, hãy đặt IsActive = false thay vì xóa.");

        _db.Coupons.Remove(coupon);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
