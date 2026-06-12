using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.AddToCart;

/// <summary>Command: add a variant to the cart (UC-03). Creates the cart if needed. Returns the updated cart.</summary>
public record AddToCartCommand(Guid VariantId, int Quantity, string? SessionId) : IRequest<CartDto>;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddToCartCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        CartHelper.EnsureOwner(_currentUser.UserId, request.SessionId);

        var variant = await _db.ProductVariants
            .Include(v => v.Inventory)
            .FirstOrDefaultAsync(v => v.Id == request.VariantId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductVariant), request.VariantId);

        if (!variant.IsActive)
            throw new ConflictException("Sản phẩm hiện không bán.");

        var cart = await CartHelper.OwnedBy(_db, _currentUser.UserId, request.SessionId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            cart = new Domain.Entities.Cart
            {
                UserId = _currentUser.UserId,
                SessionId = _currentUser.UserId is null ? request.SessionId : null,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            _db.Carts.Add(cart);
        }

        var existing = cart.Items.FirstOrDefault(i => i.VariantId == request.VariantId);
        var newQty = (existing?.Quantity ?? 0) + request.Quantity;

        // BR-01: never allow ordered qty above availability.
        var available = variant.Inventory?.Available ?? 0;
        if (newQty > available)
            throw new ConflictException($"Chỉ còn {available} sản phẩm trong kho.");

        if (existing is null)
            cart.Items.Add(new CartItem { VariantId = request.VariantId, Quantity = request.Quantity });
        else
            existing.Quantity = newQty;

        cart.ExpiresAt = DateTime.UtcNow.AddDays(30);
        await _db.SaveChangesAsync(cancellationToken);

        return await CartHelper.BuildDtoAsync(_db, cart.Id, cancellationToken);
    }
}
