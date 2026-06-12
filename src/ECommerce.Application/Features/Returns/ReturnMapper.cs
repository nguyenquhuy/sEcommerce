using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Returns;

internal static class ReturnMapper
{
    public static async Task<ReturnRequestDto> BuildDtoAsync(IAppDbContext db, Guid returnId, CancellationToken ct)
        => await db.ReturnRequests
            .AsNoTracking()
            .Where(r => r.Id == returnId)
            .Select(r => new ReturnRequestDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                OrderNumber = r.Order.OrderNumber,
                Status = r.Status,
                Reason = r.Reason,
                StaffNote = r.StaffNote,
                RefundAmount = r.RefundAmount,
                CreatedAt = r.CreatedAt,
                Items = r.Items.Select(i => new ReturnItemDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductName = i.OrderItem.ProductNameSnapshot,
                    Quantity = i.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("ReturnRequest", returnId);
}
