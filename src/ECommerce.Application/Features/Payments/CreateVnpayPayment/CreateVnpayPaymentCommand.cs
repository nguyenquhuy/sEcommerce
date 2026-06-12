using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.CreateVnpayPayment;

/// <summary>Command: build a VNPay sandbox payment URL for a pending order (§8.3 step 1-4).</summary>
public record CreateVnpayPaymentCommand(Guid OrderId, string IpAddress) : IRequest<string>;

public class CreateVnpayPaymentCommandHandler : IRequestHandler<CreateVnpayPaymentCommand, string>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IVnpayService _vnpay;

    public CreateVnpayPaymentCommandHandler(IAppDbContext db, ICurrentUserService currentUser, IVnpayService vnpay)
    {
        _db = db;
        _currentUser = currentUser;
        _vnpay = vnpay;
    }

    public async Task<string> Handle(CreateVnpayPaymentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Cần đăng nhập.");

        var order = await _db.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        if (order.Status != OrderStatus.Pending)
            throw new ConflictException("Đơn hàng không ở trạng thái chờ thanh toán.");

        if (order.PaymentMethod != PaymentMethod.VnPay)
            throw new ConflictException("Đơn hàng không dùng phương thức VNPay.");

        var payment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault()
            ?? throw new ConflictException("Đơn hàng chưa có bản ghi thanh toán.");

        // TxnRef is the idempotency key for the IPN callback (BR-13). Unique per attempt.
        var txnRef = $"{order.OrderNumber.Replace("-", "")}{DateTime.UtcNow:HHmmss}";
        payment.TxnRef = txnRef;
        payment.Status = PaymentStatus.Processing;
        await _db.SaveChangesAsync(cancellationToken);

        return _vnpay.CreatePaymentUrl(new VnpayCreateRequest(
            txnRef,
            order.Total,
            $"Thanh toan don hang {order.OrderNumber}",
            request.IpAddress));
    }
}
