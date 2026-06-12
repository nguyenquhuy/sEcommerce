namespace ECommerce.Application.Common.Interfaces;

/// <summary>Sends transactional emails (verify, reset, order notifications — BR-15).</summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
