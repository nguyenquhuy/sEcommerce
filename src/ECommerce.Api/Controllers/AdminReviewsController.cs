using ECommerce.Application.Features.Reviews;
using ECommerce.Application.Features.Reviews.ListPendingReviews;
using ECommerce.Application.Features.Reviews.ModerateReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = Roles.StaffOrAdmin)]
public class AdminReviewsController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminReviewsController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/reviews/pending — reviews awaiting moderation.</summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> Pending(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ListPendingReviewsQuery(), cancellationToken));

    /// <summary>POST /api/admin/reviews/{id}/moderate — approve/unapprove a review.</summary>
    [HttpPost("{id:guid}/moderate")]
    public async Task<IActionResult> Moderate(Guid id, ModerateRequest body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ModerateReviewCommand(id, body.Approve), cancellationToken);
        return NoContent();
    }

    public record ModerateRequest(bool Approve);
}
