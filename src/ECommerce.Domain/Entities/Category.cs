using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Product category (supports a tree via <see cref="ParentId"/>). Maps to CATEGORIES.</summary>
public class Category : AuditableEntity
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
}
