using ECommerce.Application.Common;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.Register;

/// <summary>Command: register a new customer account (UC-05). Returns the new user Id.</summary>
public record RegisterCommand(string Email, string Password, string FullName, string? Phone) : IRequest<Guid>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailService _email;

    public RegisterCommandHandler(IAppDbContext db, IPasswordHasher hasher, IEmailService email)
    {
        _db = db;
        _hasher = hasher;
        _email = email;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
            throw new ConflictException("Email đã được đăng ký.");

        var verifyToken = SecureTokenGenerator.Generate();
        var user = new User
        {
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            FullName = request.FullName.Trim(),
            Phone = request.Phone,
            Role = "Customer",
            IsEmailVerified = false,        // BR-17: must verify before login
            EmailVerifyToken = verifyToken,
            EmailVerifyExpiry = DateTime.UtcNow.AddHours(24),
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        await _email.SendAsync(
            email,
            "Xác thực tài khoản",
            $"Chào {user.FullName}, vui lòng xác thực email bằng token: <b>{verifyToken}</b> (hết hạn sau 24h).",
            cancellationToken);

        return user.Id;
    }
}
