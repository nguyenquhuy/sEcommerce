namespace ECommerce.Infrastructure.Identity;

/// <summary>Bound from configuration section "Jwt".</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ECommerce";
    public string Audience { get; set; } = "ECommerce";
    public int AccessTokenExpiryMinutes { get; set; } = 120;
}
