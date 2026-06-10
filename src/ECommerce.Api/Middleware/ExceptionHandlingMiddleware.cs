using System.Net;
using System.Text.Json;
using ECommerce.Application.Common.Exceptions;
using FluentValidation;

namespace ECommerce.Api.Middleware;

/// <summary>Maps domain/validation exceptions to proper HTTP responses.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(context, HttpStatusCode.BadRequest, "Validation failed",
                ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteAsync(HttpContext context, HttpStatusCode status, string title, object? errors = null)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new { status = (int)status, title, errors });
        return context.Response.WriteAsync(json);
    }
}
