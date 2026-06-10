using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Payment record for an order. TxnRef = idempotency key (BR-13). Maps to PAYMENTS.</summary>
public class Payment : AuditableEntity
{
    public Guid OrderId { get; set; }
    public string Method { get; set; } = string.Empty; // COD | VNPay | MoMo
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? TxnRef { get; set; }
    public string? GatewayResponseJson { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
