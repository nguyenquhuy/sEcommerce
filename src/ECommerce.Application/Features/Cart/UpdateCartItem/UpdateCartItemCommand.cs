using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.UpdateCartItem;

/// <summary>Command: set the quantity of a cart line (UC-04). Quantity 0 removes the line.</summary>
public record UpdateCartItemCommand(Guid ItemId, int Quantity, string? SessionId) : IRequest<CartDto>;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var cart = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId)
            ?? throw new NotFoundException("Không tìm thấy sản phẩm trong giỏ.");

        if (request.Quantity == 0)
        {
            _db.CartItems.Remove(item);
        }
        else
        {
            var available = await _db.Inventories
                .Where(inv => inv.VariantId == item.VariantId)
                .Select(inv => (int?)inv.Available)
                .FirstOrDefaultAsync(cancellationToken) ?? 0;

            if (request.Quantity > available) // BR-01
                throw new ConflictException($"Chỉ còn {available} sản phẩm trong kho.");

            item.Quantity = request.Quantity;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
    }
}
