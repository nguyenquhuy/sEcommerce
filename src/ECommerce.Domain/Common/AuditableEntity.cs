namespace ECommerce.Domain.Common;

/// <summary>Adds creator/modifier audit columns (CREATED_AT/BY, UPDATED_AT/BY).</summary>
public abstract class AuditableEntity : BaseEntity, IAuditable
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
