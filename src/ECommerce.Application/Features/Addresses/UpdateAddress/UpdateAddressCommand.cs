using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Addresses.UpdateAddress;

/// <summary>Command: update one of the current user's addresses (UC-13).</summary>
public record UpdateAddressCommand(
    Guid Id, string RecipientName, string Phone, string Province, string District, string? Ward, string Street, bool IsDefault)
    : IRequest;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateAddressCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Address), request.Id);

        if (address.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền sửa địa chỉ này.");

        if (request.IsDefault && !address.IsDefault)
        {
            var defaults = await _db.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync(cancellationToken);
            foreach (var a in defaults) a.IsDefault = false;
        }

        address.RecipientName = request.RecipientName;
        address.Phone = request.Phone;
        address.Province = request.Province;
        address.District = request.District;
        address.Ward = request.Ward;
        address.Street = request.Street;
        address.IsDefault = request.IsDefault;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
