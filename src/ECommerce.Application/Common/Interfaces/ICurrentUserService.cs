namespace ECommerce.Application.Common.Interfaces;

/// <summary>Exposes the authenticated user (from the JWT) to the Application layer.</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
