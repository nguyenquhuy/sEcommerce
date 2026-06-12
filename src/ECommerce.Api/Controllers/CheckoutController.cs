using ECommerce.Application.Features.Checkout;
using ECommerce.Application.Features.Checkout.ConfirmOrder;
using ECommerce.Application.Features.Checkout.StartCheckout;
using ECommerce.Application.Features.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ISender _mediator;

    public CheckoutController(ISender mediator) => _mediator = mediator;

    /// <summary>POST /api/checkout/start — reserve stock (15 min) and get the summary + shipping options.</summary>
    [HttpPost("start")]
    public async Task<ActionResult<CheckoutSummaryDto>> Start(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new StartCheckoutCommand(), cancellationToken));

    /// <summary>POST /api/checkout/confirm — place the order.</summary>
    [HttpPost("confirm")]
    public async Task<ActionResult<OrderDto>> Confirm(ConfirmOrderCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));
}
