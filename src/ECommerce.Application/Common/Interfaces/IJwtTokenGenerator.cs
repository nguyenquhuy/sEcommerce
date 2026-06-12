namespace ECommerce.Application.Common.Interfaces;

/// <summary>Issues signed JWT access tokens for authenticated users.</summary>
public interface IJwtTokenGenerator
{
    TokenResult Generate(Guid userId, string email, string role);
}

/// <summary>A signed access token and its UTC expiry.</summary>
public record TokenResult(string AccessToken, DateTime ExpiresAtUtc);
