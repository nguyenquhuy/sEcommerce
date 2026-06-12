using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>A customer's request to return items from a delivered order (UC-11, §8.5). Maps to RETURN_REQUESTS.</summary>
public class ReturnRequest : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected/Received/Refunded
    public string Reason { get; set; } = string.Empty;
    public string? StaffNote { get; set; }
    public decimal RefundAmount { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
}
