namespace ECommerce.Application.Features.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public int LoyaltyPoint { get; set; }
    public DateTime CreatedAt { get; set; }
}
