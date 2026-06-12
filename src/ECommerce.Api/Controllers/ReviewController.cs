using ECommerce.Application.Features.Reviews;
using ECommerce.Application.Features.Reviews.CreateReview;
using ECommerce.Application.Features.Reviews.GetProductReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly ISender _mediator;

    public ReviewController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/review/product/{productId} — approved reviews for a product (public).</summary>
    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> ForProduct(Guid productId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetProductReviewsQuery(productId), cancellationToken));

    /// <summary>POST /api/review — review a purchased item (UC-12).</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create(CreateReviewCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));
}
