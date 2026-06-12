using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>A line within a return request, referencing the original order item. Maps to RETURN_ITEMS.</summary>
public class ReturnItem : AuditableEntity
{
    public Guid ReturnRequestId { get; set; }
    public Guid OrderItemId { get; set; }
    public int Quantity { get; set; }

    // Navigation
    public ReturnRequest ReturnRequest { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}
