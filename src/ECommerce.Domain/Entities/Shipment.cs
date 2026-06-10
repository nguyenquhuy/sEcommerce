using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Delivery for an order via GHTK/GHN. Maps to SHIPMENTS.</summary>
public class Shipment : AuditableEntity
{
    public Guid OrderId { get; set; }
    public string Provider { get; set; } = string.Empty; // GHTK | GHN
    public string? TrackingNumber { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public decimal? CostFee { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
