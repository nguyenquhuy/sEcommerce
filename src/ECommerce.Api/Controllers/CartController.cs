using ECommerce.Application.Features.Cart;
using ECommerce.Application.Features.Cart.AddToCart;
using ECommerce.Application.Features.Cart.ApplyCoupon;
using ECommerce.Application.Features.Cart.GetCart;
using ECommerce.Application.Features.Cart.MergeCart;
using ECommerce.Application.Features.Cart.RemoveCartItem;
using ECommerce.Application.Features.Cart.RemoveCoupon;
using ECommerce.Application.Features.Cart.UpdateCartItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Cart endpoints. Logged-in users are resolved from the JWT; guests must send a stable
/// session id in the <c>X-Cart-Session</c> header.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private const string SessionHeader = "X-Cart-Session";
    private readonly ISender _mediator;

    public CartController(ISender mediator) => _mediator = mediator;

    private string? Session => Request.Headers.TryGetValue(SessionHeader, out var v) ? v.ToString() : null;

    /// <summary>GET /api/cart — current cart.</summary>
    [HttpGet]
    public async Task<ActionResult<CartDto>> Get(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetCartQuery(Session), cancellationToken));

    /// <summary>POST /api/cart/items — add a variant.</summary>
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(AddItemRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new AddToCartCommand(body.VariantId, body.Quantity, Session), cancellationToken));

    /// <summary>PUT /api/cart/items/{itemId} — set quantity (0 removes).</summary>
    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid itemId, QuantityRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new UpdateCartItemCommand(itemId, body.Quantity, Session), cancellationToken));

    /// <summary>DELETE /api/cart/items/{itemId} — remove a line.</summary>
    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid itemId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new RemoveCartItemCommand(itemId, Session), cancellationToken));

    /// <summary>POST /api/cart/coupon — apply a coupon code.</summary>
    [HttpPost("coupon")]
    public async Task<ActionResult<CartDto>> ApplyCoupon(CouponRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ApplyCouponCommand(body.Code, Session), cancellationToken));

    /// <summary>DELETE /api/cart/coupon — clear the coupon.</summary>
    [HttpDelete("coupon")]
    public async Task<ActionResult<CartDto>> RemoveCoupon(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new RemoveCouponCommand(Session), cancellationToken));

    /// <summary>POST /api/cart/merge — merge the guest session cart into the logged-in user's cart.</summary>
    [HttpPost("merge")]
    public async Task<ActionResult<CartDto>> Merge(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Session))
            return BadRequest($"Thiếu header {SessionHeader}.");
        return Ok(await _mediator.Send(new MergeCartCommand(Session!), cancellationToken));
    }

    public record AddItemRequest(Guid VariantId, int Quantity);
    public record QuantityRequest(int Quantity);
    public record CouponRequest(string Code);
}
