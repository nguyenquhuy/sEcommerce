using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Order: a snapshot of the cart at checkout. Maps to ORDERS.</summary>
public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? CouponCode { get; set; }
    public string ShippingAddressJson { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<OrderAuditLog> AuditLogs { get; set; } = new List<OrderAuditLog>();
}
