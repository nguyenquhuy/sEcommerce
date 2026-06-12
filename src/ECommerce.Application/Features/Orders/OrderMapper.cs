using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders;

internal static class OrderMapper
{
    /// <summary>Loads a full order (items + payment status) and projects it to <see cref="OrderDto"/>.</summary>
    public static async Task<OrderDto> BuildDtoAsync(IAppDbContext db, Guid orderId, CancellationToken ct)
    {
        var dto = await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                Status = o.Status,
                Subtotal = o.Subtotal,
                ShippingFee = o.ShippingFee,
                Discount = o.Discount,
                Total = o.Total,
                CouponCode = o.CouponCode,
                ShippingAddressJson = o.ShippingAddressJson,
                Note = o.Note,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => p.Status)
                    .FirstOrDefault(),
                CreatedAt = o.CreatedAt,
                ConfirmedAt = o.ConfirmedAt,
                ShippedAt = o.ShippedAt,
                DeliveredAt = o.DeliveredAt,
                CompletedAt = o.CompletedAt,
                CancelledAt = o.CancelledAt,
                CancelReason = o.CancelReason,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    VariantId = i.VariantId,
                    ProductName = i.ProductNameSnapshot,
                    Sku = i.VariantSkuSnapshot,
                    AttributesJson = i.VariantAttributesSnapshot,
                    UnitPrice = i.UnitPriceSnapshot,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Order", orderId);

        return dto;
    }
}
