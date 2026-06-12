using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reviews.GetProductReviews;

/// <summary>Query: approved reviews for a product (public).</summary>
public record GetProductReviewsQuery(Guid ProductId) : IRequest<IReadOnlyList<ReviewDto>>;

public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IAppDbContext _db;

    public GetProductReviewsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        => await _db.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == request.ProductId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                AuthorName = r.User.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                IsApproved = r.IsApproved,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);
}
