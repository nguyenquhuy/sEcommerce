using ECommerce.Application.Features.Auth;
using ECommerce.Application.Features.Auth.ForgotPassword;
using ECommerce.Application.Features.Auth.Login;
using ECommerce.Application.Features.Auth.Register;
using ECommerce.Application.Features.Auth.ResetPassword;
using ECommerce.Application.Features.Auth.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator) => _mediator = mediator;

    /// <summary>POST /api/auth/register — create a customer account (UC-05).</summary>
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));

    /// <summary>POST /api/auth/verify-email — confirm registration with the emailed token.</summary>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>POST /api/auth/login — authenticate and receive a JWT (UC-06).</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));

    /// <summary>POST /api/auth/forgot-password — email a password-reset token.</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok(new { message = "Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi." });
    }

    /// <summary>POST /api/auth/reset-password — set a new password using the token.</summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
