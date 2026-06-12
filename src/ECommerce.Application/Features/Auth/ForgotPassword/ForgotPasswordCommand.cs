using ECommerce.Application.Common;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.ForgotPassword;

/// <summary>Command: start password reset — emails a reset token. Always succeeds (no user enumeration).</summary>
public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _email;

    public ForgotPasswordCommandHandler(IAppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null || !user.IsActive)
            return; // silently ignore to avoid leaking which emails exist

        user.PasswordResetToken = SecureTokenGenerator.Generate();
        user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
        await _db.SaveChangesAsync(cancellationToken);

        await _email.SendAsync(
            user.Email,
            "Đặt lại mật khẩu",
            $"Token đặt lại mật khẩu của bạn: <b>{user.PasswordResetToken}</b> (hết hạn sau 1 giờ).",
            cancellationToken);
    }
}
