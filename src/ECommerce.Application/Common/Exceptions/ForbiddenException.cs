namespace ECommerce.Application.Common.Exceptions;

/// <summary>Thrown when an authenticated user lacks permission for an action. Mapped to HTTP 403 by the API.</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
