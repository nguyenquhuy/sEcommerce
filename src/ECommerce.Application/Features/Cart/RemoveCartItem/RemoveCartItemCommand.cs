using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.RemoveCartItem;

/// <summary>Command: remove a line from the cart (UC-04).</summary>
public record RemoveCartItemCommand(Guid ItemId, string? SessionId) : IRequest<CartDto>;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveCartItemCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var cart = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId)
            ?? throw new NotFoundException("Không tìm thấy sản phẩm trong giỏ.");

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);

        return await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
    }
}
