using ECommerce.Application.Features.Catalog.AdjustInventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ISender _mediator;

    public InventoryController(ISender mediator) => _mediator = mediator;

    /// <summary>POST /api/inventory/{variantId}/adjust — change on-hand stock by a delta (UC-24). Returns new OnHand.</summary>
    [HttpPost("{variantId:guid}/adjust")]
    public async Task<ActionResult<int>> Adjust(
        Guid variantId, AdjustInventoryRequest request, CancellationToken cancellationToken)
    {
        var newOnHand = await _mediator.Send(
            new AdjustInventoryCommand(variantId, request.Delta, request.Reason), cancellationToken);
        return Ok(new { variantId, onHand = newOnHand });
    }

    /// <summary>Request body for an inventory adjustment.</summary>
    public record AdjustInventoryRequest(int Delta, string? Reason);
}
