using ECommerce.Application.Features.Returns;
using ECommerce.Application.Features.Returns.RequestReturn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReturnController : ControllerBase
{
    private readonly ISender _mediator;

    public ReturnController(ISender mediator) => _mediator = mediator;

    /// <summary>POST /api/return — request a return for delivered items (UC-11).</summary>
    [HttpPost]
    public async Task<ActionResult<ReturnRequestDto>> Create(RequestReturnCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));
}
