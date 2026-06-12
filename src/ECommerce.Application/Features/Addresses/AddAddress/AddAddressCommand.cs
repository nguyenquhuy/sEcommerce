using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Addresses.AddAddress;

/// <summary>Command: add an address for the current user (UC-13). Returns the new Id.</summary>
public record AddAddressCommand(
    string RecipientName, string Phone, string Province, string District, string? Ward, string Street, bool IsDefault)
    : IRequest<Guid>;

public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, Guid>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddAddressCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var isFirst = !await _db.Addresses.AnyAsync(a => a.UserId == userId, cancellationToken);
        var makeDefault = request.IsDefault || isFirst;

        if (makeDefault)
            await UnsetExistingDefaultAsync(userId, cancellationToken);

        var address = new Address
        {
            UserId = userId,
            RecipientName = request.RecipientName,
            Phone = request.Phone,
            Province = request.Province,
            District = request.District,
            Ward = request.Ward,
            Street = request.Street,
            IsDefault = makeDefault
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(cancellationToken);
        return address.Id;
    }

    private async Task UnsetExistingDefaultAsync(Guid userId, CancellationToken ct)
    {
        var defaults = await _db.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync(ct);
        foreach (var a in defaults) a.IsDefault = false;
    }
}
