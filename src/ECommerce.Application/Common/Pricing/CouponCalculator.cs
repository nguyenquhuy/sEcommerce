using ECommerce.Application.Common.Exceptions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Pricing;

/// <summary>Validates coupons (BR-20) and computes their discount against a subtotal.</summary>
public static class CouponCalculator
{
    /// <summary>Discount amount for a coupon against a subtotal (never exceeds the subtotal).</summary>
    public static decimal ComputeDiscount(Coupon coupon, decimal subtotal)
    {
        decimal discount = coupon.Type switch
        {
            "Percent" => subtotal * coupon.Value / 100m,
            "Fixed" => coupon.Value,
            _ => 0m
        };

        if (coupon.Type == "Percent" && coupon.MaxDiscountAmount is { } cap && discount > cap)
            discount = cap;

        if (discount > subtotal)
            discount = subtotal;

        return Math.Round(discount, 0); // VND — whole đồng
    }

    /// <summary>Throws ConflictException if the coupon cannot be applied to this subtotal at this time (BR-20).</summary>
    public static void EnsureApplicable(Coupon coupon, decimal subtotal, DateTime nowUtc)
    {
        if (!coupon.IsActive)
            throw new ConflictException($"Mã '{coupon.Code}' không còn hiệu lực.");

        if (nowUtc < coupon.StartAt || nowUtc > coupon.EndAt)
            throw new ConflictException($"Mã '{coupon.Code}' đã hết hạn hoặc chưa bắt đầu.");

        if (coupon.MaxUsage is { } max && coupon.UsedCount >= max)
            throw new ConflictException($"Mã '{coupon.Code}' đã hết lượt sử dụng.");

        if (subtotal < coupon.MinOrderAmount)
            throw new ConflictException(
                $"Đơn hàng tối thiểu {coupon.MinOrderAmount:N0}đ để dùng mã '{coupon.Code}'.");
    }
}
