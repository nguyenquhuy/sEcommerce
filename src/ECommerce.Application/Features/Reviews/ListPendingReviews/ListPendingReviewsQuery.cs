using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reviews.ListPendingReviews;

/// <summary>Query: reviews awaiting moderation (admin).</summary>
public record ListPendingReviewsQuery : IRequest<IReadOnlyList<ReviewDto>>;

public class ListPendingReviewsQueryHandler : IRequestHandler<ListPendingReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IAppDbContext _db;

    public ListPendingReviewsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ReviewDto>> Handle(ListPendingReviewsQuery request, CancellationToken cancellationToken)
        => await _db.Reviews
            .AsNoTracking()
            .Where(r => !r.IsApproved)
            .OrderBy(r => r.CreatedAt)
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
