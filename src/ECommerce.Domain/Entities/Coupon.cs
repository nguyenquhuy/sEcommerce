using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Discount code with rules. Maps to COUPONS.</summary>
public class Coupon : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Percent | Fixed
    public decimal Value { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? MaxUsage { get; set; }
    public int MaxUsagePerUser { get; set; } = 1;
    public int UsedCount { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsActive { get; set; } = true;
}
