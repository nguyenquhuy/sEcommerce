namespace ECommerce.Domain.Common;

/// <summary>Base for all entities with a GUID primary key.</summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
}
