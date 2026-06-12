using ECommerce.Application.Features.Returns;
using ECommerce.Application.Features.Returns.ListReturns;
using ECommerce.Application.Features.Returns.ProcessReturn;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/returns")]
[Authorize(Roles = Roles.StaffOrAdmin)]
public class AdminReturnsController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminReturnsController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/returns — return requests, optional ?status= filter.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReturnRequestDto>>> List(
        [FromQuery] string? status, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ListReturnsQuery(status), cancellationToken));

    /// <summary>POST /api/admin/returns/{id}/process — Approve/Reject/Receive/Refund.</summary>
    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<ReturnRequestDto>> Process(Guid id, ProcessRequest body, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ProcessReturnCommand(id, body.Action, body.StaffNote), cancellationToken));

    public record ProcessRequest(string Action, string? StaffNote);
}
