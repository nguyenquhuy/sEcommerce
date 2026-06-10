using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Order status-change history (BR-14). Has its own ChangedBy/ChangedAt, so not AuditableEntity. Maps to ORDER_AUDIT_LOGS.</summary>
public class OrderAuditLog : BaseEntity
{
    public Guid OrderId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public Guid? ChangedBy { get; set; } // null = system
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
