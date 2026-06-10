using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Product review tied to a purchased OrderItem (BR-11). Maps to REVIEWS.</summary>
public class Review : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderItemId { get; set; }
    public byte Rating { get; set; } // 1..5
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}
