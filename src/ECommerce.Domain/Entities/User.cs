using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>Account: Customer / Staff / Admin. Maps to USERS.</summary>
public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "Customer";
    public bool IsEmailVerified { get; set; }
    public string? EmailVerifyToken { get; set; }
    public DateTime? EmailVerifyExpiry { get; set; }
    public bool IsActive { get; set; } = true;
    public int LoyaltyPoint { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
