using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reviews.CreateReview;

/// <summary>Command: review a purchased item (UC-12, BR-11). Returns the new review Id (pending moderation).</summary>
public record CreateReviewCommand(Guid OrderItemId, byte Rating, string? Comment) : IRequest<Guid>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var line = await _db.OrderItems
            .Include(i => i.Order)
            .Include(i => i.Variant)
            .FirstOrDefaultAsync(i => i.Id == request.OrderItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(OrderItem), request.OrderItemId);

        if (line.Order.UserId != userId)
            throw new ForbiddenException("Bạn chỉ có thể đánh giá sản phẩm mình đã mua.");

        // BR-11: only after the order is completed.
        if (line.Order.Status != OrderStatus.Completed)
            throw new ConflictException("Chỉ đánh giá được khi đơn hàng đã hoàn tất.");

        if (await _db.Reviews.AnyAsync(r => r.OrderItemId == request.OrderItemId, cancellationToken))
            throw new ConflictException("Sản phẩm này trong đơn đã được đánh giá.");

        var review = new Review
        {
            ProductId = line.Variant.ProductId,
            UserId = userId,
            OrderItemId = request.OrderItemId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsApproved = false // BR-25 moderation
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(cancellationToken);
        return review.Id;
    }
}
