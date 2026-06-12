using System.Security.Cryptography;

namespace ECommerce.Application.Common;

/// <summary>Generates URL-safe random tokens for email verification / password reset.</summary>
public static class SecureTokenGenerator
{
    public static string Generate(int byteLength = 32)
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(byteLength)).ToLowerInvariant();
}
