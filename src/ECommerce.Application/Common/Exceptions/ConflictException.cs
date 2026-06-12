namespace ECommerce.Application.Common.Exceptions;

/// <summary>Thrown when a request conflicts with current state (e.g. duplicate slug/SKU). Mapped to HTTP 409 by the API.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
