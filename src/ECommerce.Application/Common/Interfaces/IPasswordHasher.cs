namespace ECommerce.Application.Common.Interfaces;

/// <summary>Hashes and verifies passwords (BCrypt — see security rules in §7.3).</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
