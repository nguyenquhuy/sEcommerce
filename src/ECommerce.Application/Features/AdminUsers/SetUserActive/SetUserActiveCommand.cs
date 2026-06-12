using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.AdminUsers.SetUserActive;

/// <summary>Command: lock/unlock a user account (UC-33).</summary>
public record SetUserActiveCommand(Guid Id, bool IsActive) : IRequest;

public class SetUserActiveCommandHandler : IRequestHandler<SetUserActiveCommand>
{
    private readonly IAppDbContext _db;

    public SetUserActiveCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
