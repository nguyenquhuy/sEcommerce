namespace ECommerce.Domain.Common;

/// <summary>Audit columns shared by entities (incl. those that don't use BaseEntity, e.g. Inventory).</summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}
