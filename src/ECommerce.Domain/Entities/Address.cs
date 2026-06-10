using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Shipping address belonging to a user. Maps to ADDRESSES.</summary>
public class Address : AuditableEntity
{
    public Guid UserId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string Street { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
