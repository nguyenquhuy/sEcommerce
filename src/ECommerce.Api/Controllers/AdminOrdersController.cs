using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders;
using ECommerce.Application.Features.Orders.AdminListOrders;
using ECommerce.Application.Features.Orders.ChangeOrderStatus;
using ECommerce.Application.Features.Orders.CreateShipment;
using ECommerce.Application.Features.Orders.GetOrderById;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = Roles.StaffOrAdmin)]
public class AdminOrdersController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminOrdersController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/orders — all orders, filter by status/keyword (UC-20).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderListItemDto>>> List(
        [FromQuery] AdminListOrdersQuery query, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(query, cancellationToken));

    /// <summary>GET /api/admin/orders/{id} — order detail.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken));

    /// <summary>POST /api/admin/orders/{id}/status — advance the order state machine (UC-21).</summary>
    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> ChangeStatus(Guid id, ChangeStatusRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ChangeOrderStatusCommand(id, body.ToStatus, body.Reason), cancellationToken));

    /// <summary>POST /api/admin/orders/{id}/shipment — create a shipment, moving the order to Shipping (UC-22).</summary>
    [HttpPost("{id:guid}/shipment")]
    public async Task<ActionResult<OrderDto>> CreateShipment(Guid id, ShipmentRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new CreateShipmentCommand(id, body.Provider, body.TrackingNumber), cancellationToken));

    public record ChangeStatusRequest(string ToStatus, string? Reason);
    public record ShipmentRequest(string Provider, string? TrackingNumber);
}
