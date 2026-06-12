using ECommerce.Application.Features.Profile;
using ECommerce.Application.Features.Profile.ChangePassword;
using ECommerce.Application.Features.Profile.GetMyProfile;
using ECommerce.Application.Features.Profile.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ISender _mediator;

    public ProfileController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/profile — my profile.</summary>
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMyProfileQuery(), cancellationToken));

    /// <summary>PUT /api/profile — update my name/phone.</summary>
    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>POST /api/profile/change-password — change my password.</summary>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
