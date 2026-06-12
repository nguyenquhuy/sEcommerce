using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.AdminUsers.ChangeUserRole;

/// <summary>Command: change a user's role (UC-33).</summary>
public record ChangeUserRoleCommand(Guid Id, string Role) : IRequest;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Role).Must(r => r is Roles.Customer or Roles.Staff or Roles.Admin)
            .WithMessage("Role phải là Customer, Staff hoặc Admin.");
    }
}

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IAppDbContext _db;

    public ChangeUserRoleCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.Role = request.Role;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
