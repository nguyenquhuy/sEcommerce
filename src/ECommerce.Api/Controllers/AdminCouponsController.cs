using ECommerce.Application.Features.Coupons;
using ECommerce.Application.Features.Coupons.CreateCoupon;
using ECommerce.Application.Features.Coupons.DeleteCoupon;
using ECommerce.Application.Features.Coupons.GetCoupons;
using ECommerce.Application.Features.Coupons.UpdateCoupon;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/coupons")]
[Authorize(Roles = Roles.Admin)]
public class AdminCouponsController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminCouponsController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/coupons — list all coupons.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CouponDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetCouponsQuery(), cancellationToken));

    /// <summary>POST /api/admin/coupons — create a coupon.</summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCouponCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));

    /// <summary>PUT /api/admin/coupons/{id} — update a coupon.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCouponCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id and body id mismatch.");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/admin/coupons/{id} — delete a coupon.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCouponCommand(id), cancellationToken);
        return NoContent();
    }
}
