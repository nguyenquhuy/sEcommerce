using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Addresses.GetMyAddresses;

/// <summary>Query: the current user's saved addresses (UC-13).</summary>
public record GetMyAddressesQuery : IRequest<IReadOnlyList<AddressDto>>;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, IReadOnlyList<AddressDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyAddressesQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AddressDto>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");
        return await _db.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault).ThenBy(a => a.RecipientName)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                RecipientName = a.RecipientName,
                Phone = a.Phone,
                Province = a.Province,
                District = a.District,
                Ward = a.Ward,
                Street = a.Street,
                IsDefault = a.IsDefault
            })
            .ToListAsync(cancellationToken);
    }
}
