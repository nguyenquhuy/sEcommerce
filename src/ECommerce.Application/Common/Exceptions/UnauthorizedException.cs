namespace ECommerce.Application.Common.Exceptions;

/// <summary>Thrown when credentials are missing/invalid. Mapped to HTTP 401 by the API.</summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
