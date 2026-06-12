using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Addresses.DeleteAddress;

/// <summary>Command: delete one of the current user's addresses (UC-13).</summary>
public record DeleteAddressCommand(Guid Id) : IRequest;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteAddressCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Address), request.Id);

        if (address.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền xóa địa chỉ này.");

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
