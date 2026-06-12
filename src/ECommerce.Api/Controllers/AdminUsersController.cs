using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.AdminUsers;
using ECommerce.Application.Features.AdminUsers.ChangeUserRole;
using ECommerce.Application.Features.AdminUsers.ListUsers;
using ECommerce.Application.Features.AdminUsers.SetUserActive;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = Roles.Admin)]
public class AdminUsersController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminUsersController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/users — paged user list (UC-33).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserSummaryDto>>> List(
        [FromQuery] ListUsersQuery query, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(query, cancellationToken));

    /// <summary>PUT /api/admin/users/{id}/role — change a user's role.</summary>
    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, RoleRequest body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ChangeUserRoleCommand(id, body.Role), cancellationToken);
        return NoContent();
    }

    /// <summary>PUT /api/admin/users/{id}/active — lock/unlock a user.</summary>
    [HttpPut("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, ActiveRequest body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetUserActiveCommand(id, body.IsActive), cancellationToken);
        return NoContent();
    }

    public record RoleRequest(string Role);
    public record ActiveRequest(bool IsActive);
}
