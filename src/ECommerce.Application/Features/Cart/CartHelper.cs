using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Pricing;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart;

/// <summary>Shared logic for locating a cart (member or guest) and projecting it to <see cref="CartDto"/>.</summary>
internal static class CartHelper
{
    /// <summary>A cart must belong to either a logged-in user or a guest session.</summary>
    public static void EnsureOwner(Guid? userId, string? sessionId)
    {
        if (userId is null && string.IsNullOrWhiteSpace(sessionId))
            throw new UnauthorizedException("Cần đăng nhập hoặc cung cấp session giỏ hàng (header X-Cart-Session).");
    }

    public static IQueryable<Domain.Entities.Cart> OwnedBy(IAppDbContext db, Guid? userId, string? sessionId)
        => userId is not null
            ? db.Carts.Where(c => c.UserId == userId)
            : db.Carts.Where(c => c.SessionId == sessionId);

    /// <summary>Loads a cart with all data needed for display and builds the money-computed DTO.</summary>
    public static async Task<CartDto> BuildDtoAsync(IAppDbContext db, Guid cartId, CancellationToken ct)
    {
        var cart = await db.Carts
            .AsNoTracking()
            .Where(c => c.Id == cartId)
            .Select(c => new
            {
                c.Id,
                c.CouponId,
                Items = c.Items.Select(i => new
                {
                    i.Id,
                    i.VariantId,
                    i.Quantity,
                    i.Variant.Sku,
                    i.Variant.Price,
                    i.Variant.AttributesJson,
                    i.Variant.ImageUrl,
                    ProductId = i.Variant.ProductId,
                    ProductName = i.Variant.Product.Name,
                    ProductSlug = i.Variant.Product.Slug,
                    Available = i.Variant.Inventory != null ? i.Variant.Inventory.Available : 0
                }).ToList()
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Cart", cartId);

        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            VariantId = i.VariantId,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductSlug = i.ProductSlug,
            Sku = i.Sku,
            AttributesJson = i.AttributesJson,
            ImageUrl = i.ImageUrl,
            UnitPrice = i.Price,
            Quantity = i.Quantity,
            LineTotal = i.Price * i.Quantity,
            Available = i.Available,
            IsOutOfStock = i.Quantity > i.Available
        }).ToList();

        var subtotal = items.Sum(i => i.LineTotal);

        decimal discount = 0m;
        string? couponCode = null;
        if (cart.CouponId is { } couponId)
        {
            var coupon = await db.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == couponId, ct);
            if (coupon is not null)
            {
                couponCode = coupon.Code;
                discount = CouponCalculator.ComputeDiscount(coupon, subtotal);
            }
        }

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            TotalQuantity = items.Sum(i => i.Quantity),
            Subtotal = subtotal,
            CouponCode = couponCode,
            Discount = discount,
            Total = subtotal - discount
        };
    }
}
