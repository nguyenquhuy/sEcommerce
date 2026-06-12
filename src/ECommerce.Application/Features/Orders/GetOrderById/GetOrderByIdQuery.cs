using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.GetOrderById;

/// <summary>Query: order detail. Customers can only see their own; staff/admin can see any.</summary>
public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetOrderByIdQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");
        var isStaff = _currentUser.Role is Roles.Staff or Roles.Admin;

        var ownerId = await _db.Orders.AsNoTracking()
            .Where(o => o.Id == request.Id)
            .Select(o => (Guid?)o.UserId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (!isStaff && ownerId != userId)
            throw new ForbiddenException("Bạn không có quyền xem đơn hàng này.");

        return await OrderMapper.BuildDtoAsync(_db, request.Id, cancellationToken);
    }
}
