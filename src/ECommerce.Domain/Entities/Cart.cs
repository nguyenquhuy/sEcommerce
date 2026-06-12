using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Shopping cart for a member (UserId) or guest (SessionId). Maps to CARTS.</summary>
public class Cart : AuditableEntity
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
    public Guid? CouponId { get; set; }
    public DateTime? ExpiresAt { get; set; }

    /// <summary>When the current stock reservation (BR-02) expires; null = no active reservation.</summary>
    public DateTime? ReservedUntil { get; set; }

    // Navigation
    public User? User { get; set; }
    public Coupon? Coupon { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
