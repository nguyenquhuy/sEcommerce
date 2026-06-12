using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Payments.CreateVnpayPayment;
using ECommerce.Application.Features.Payments.ProcessVnpayIpn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IVnpayService _vnpay;

    public PaymentController(ISender mediator, IVnpayService vnpay)
    {
        _mediator = mediator;
        _vnpay = vnpay;
    }

    /// <summary>POST /api/payment/vnpay/create — get a VNPay payment URL for a pending order.</summary>
    [HttpPost("vnpay/create")]
    [Authorize]
    public async Task<ActionResult<object>> CreateVnpay(CreateRequest body, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = await _mediator.Send(new CreateVnpayPaymentCommand(body.OrderId, ip), cancellationToken);
        return Ok(new { paymentUrl = url });
    }

    /// <summary>
    /// GET /api/payment/vnpay-ipn — VNPay server-to-server callback. This is the ONLY place
    /// the order is confirmed (never the return URL). Returns VNPay's expected RspCode/Message.
    /// </summary>
    [HttpGet("vnpay-ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayIpn(CancellationToken cancellationToken)
    {
        var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
        var result = await _mediator.Send(new ProcessVnpayIpnCommand(query), cancellationToken);
        return Ok(new { RspCode = result.RspCode, Message = result.Message });
    }

    /// <summary>
    /// GET /api/payment/vnpay-return — browser redirect after payment. Does NOT change state;
    /// just reports a friendly result. The client should poll the order for the real status.
    /// </summary>
    [HttpGet("vnpay-return")]
    [AllowAnonymous]
    public IActionResult VnpayReturn()
    {
        var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
        var valid = _vnpay.ValidateSignature(query);
        var code = query.TryGetValue("vnp_ResponseCode", out var rc) ? rc : null;

        if (!valid)
            return Ok(new { success = false, message = "Chữ ký không hợp lệ." });

        return Ok(new
        {
            success = code == "00",
            message = code == "00"
                ? "Thanh toán thành công. Đơn hàng sẽ được xác nhận trong giây lát."
                : "Thanh toán không thành công hoặc đã bị hủy.",
            responseCode = code
        });
    }

    public record CreateRequest(Guid OrderId);
}
