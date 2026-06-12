using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders;
using ECommerce.Application.Features.Orders.CancelOrder;
using ECommerce.Application.Features.Orders.GetOrderById;
using ECommerce.Application.Features.Orders.ListMyOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly ISender _mediator;

    public OrderController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/order — my order history (UC-09).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderListItemDto>>> MyOrders(
        [FromQuery] ListMyOrdersQuery query, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(query, cancellationToken));

    /// <summary>GET /api/order/{id} — order detail (own order).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken));

    /// <summary>POST /api/order/{id}/cancel — cancel my order (UC-10).</summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(Guid id, CancelRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new CancelOrderCommand(id, body?.Reason), cancellationToken));

    public record CancelRequest(string? Reason);
}
