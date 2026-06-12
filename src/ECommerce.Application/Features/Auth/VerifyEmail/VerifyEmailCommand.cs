using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.VerifyEmail;

/// <summary>Command: confirm a registration via the emailed token (UC-05 step 9-12).</summary>
public record VerifyEmailCommand(string Token) : IRequest;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly IAppDbContext _db;

    public VerifyEmailCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailVerifyToken == request.Token, cancellationToken)
            ?? throw new NotFoundException("Token xác thực không hợp lệ.");

        if (user.IsEmailVerified)
            return; // idempotent

        if (user.EmailVerifyExpiry is null || user.EmailVerifyExpiry < DateTime.UtcNow)
            throw new ConflictException("Token xác thực đã hết hạn. Vui lòng đăng ký lại hoặc yêu cầu gửi lại.");

        user.IsEmailVerified = true;
        user.EmailVerifyToken = null;
        user.EmailVerifyExpiry = null;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
