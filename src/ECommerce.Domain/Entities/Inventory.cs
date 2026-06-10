using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Stock per variant (1-1). PK is VariantId (no surrogate Id). Maps to INVENTORIES.</summary>
public class Inventory : IAuditable
{
    public Guid VariantId { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }

    /// <summary>Computed in DB: ON_HAND - RESERVED (read-only).</summary>
    public int Available { get; private set; }

    /// <summary>Optimistic concurrency token (SQL Server rowversion).</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation
    public ProductVariant Variant { get; set; } = null!;
}
