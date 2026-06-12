using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reviews.ModerateReview;

/// <summary>Command: admin approves or unapproves a review (BR-25 moderation).</summary>
public record ModerateReviewCommand(Guid Id, bool Approve) : IRequest;

public class ModerateReviewCommandHandler : IRequestHandler<ModerateReviewCommand>
{
    private readonly IAppDbContext _db;

    public ModerateReviewCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(ModerateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Review), request.Id);

        review.IsApproved = request.Approve;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
