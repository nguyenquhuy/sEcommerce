using System.Text.Json;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.ProcessVnpayIpn;

/// <summary>VNPay's expected IPN acknowledgement payload.</summary>
public record VnpayIpnResult(string RspCode, string Message);

/// <summary>
/// Command: process a VNPay server-to-server IPN (§8.3 step 11-17). Verifies the HMAC,
/// is idempotent on vnp_TxnRef (BR-13), and confirms the order on success.
/// </summary>
public record ProcessVnpayIpnCommand(IDictionary<string, string> Params) : IRequest<VnpayIpnResult>;

public class ProcessVnpayIpnCommandHandler : IRequestHandler<ProcessVnpayIpnCommand, VnpayIpnResult>
{
    private readonly IAppDbContext _db;
    private readonly IVnpayService _vnpay;

    public ProcessVnpayIpnCommandHandler(IAppDbContext db, IVnpayService vnpay)
    {
        _db = db;
        _vnpay = vnpay;
    }

    public async Task<VnpayIpnResult> Handle(ProcessVnpayIpnCommand request, CancellationToken cancellationToken)
    {
        var p = request.Params;

        if (!_vnpay.ValidateSignature(p))
            return new VnpayIpnResult("97", "Invalid signature");

        if (!p.TryGetValue("vnp_TxnRef", out var txnRef) || string.IsNullOrEmpty(txnRef))
            return new VnpayIpnResult("01", "Order not found");

        var payment = await _db.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.TxnRef == txnRef, cancellationToken);

        if (payment is null)
            return new VnpayIpnResult("01", "Order not found");

        // BR-13: idempotent — if already settled, just acknowledge.
        if (payment.Status == PaymentStatus.Success)
            return new VnpayIpnResult("00", "Order already confirmed");

        // Validate amount matches what we asked for.
        if (p.TryGetValue("vnp_Amount", out var amountRaw)
            && long.TryParse(amountRaw, out var amount)
            && amount != (long)(payment.Amount * 100))
        {
            return new VnpayIpnResult("04", "Invalid amount");
        }

        payment.GatewayResponseJson = JsonSerializer.Serialize(p);
        var responseCode = p.TryGetValue("vnp_ResponseCode", out var rc) ? rc : null;

        if (responseCode == "00")
        {
            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;

            var order = payment.Order;
            if (order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Confirmed;
                order.ConfirmedAt = DateTime.UtcNow;
                _db.OrderAuditLogs.Add(new OrderAuditLog
                {
                    OrderId = order.Id,
                    FromStatus = OrderStatus.Pending,
                    ToStatus = OrderStatus.Confirmed,
                    ChangedBy = null, // system
                    Reason = "VNPay payment success",
                    ChangedAt = DateTime.UtcNow
                });
            }
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new VnpayIpnResult("00", "Confirm Success");
    }
}
