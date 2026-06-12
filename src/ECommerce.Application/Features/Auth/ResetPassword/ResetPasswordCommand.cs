using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.ResetPassword;

/// <summary>Command: complete password reset using the emailed token.</summary>
public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public ResetPasswordCommandHandler(IAppDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token, cancellationToken)
            ?? throw new NotFoundException("Token đặt lại mật khẩu không hợp lệ.");

        if (user.PasswordResetExpiry is null || user.PasswordResetExpiry < DateTime.UtcNow)
            throw new ConflictException("Token đặt lại mật khẩu đã hết hạn.");

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetExpiry = null;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
