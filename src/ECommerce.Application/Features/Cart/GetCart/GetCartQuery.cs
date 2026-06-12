using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.GetCart;

/// <summary>Query: the current cart for the logged-in user or the guest session (UC-04).</summary>
public record GetCartQuery(string? SessionId) : IRequest<CartDto>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetCartQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var cartId = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return cartId is null
            ? new CartDto() // empty cart
            : await CartHelper.BuildDtoAsync(_db, cartId.Value, cancellationToken);
    }
}
