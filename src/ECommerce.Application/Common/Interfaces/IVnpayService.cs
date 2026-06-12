namespace ECommerce.Application.Common.Interfaces;

/// <summary>Builds VNPay payment URLs and verifies callback signatures (HMAC-SHA512).</summary>
public interface IVnpayService
{
    string CreatePaymentUrl(VnpayCreateRequest request);

    /// <summary>True if the <c>vnp_SecureHash</c> in the callback params matches a freshly computed hash.</summary>
    bool ValidateSignature(IDictionary<string, string> queryParams);
}

public record VnpayCreateRequest(string TxnRef, decimal Amount, string OrderInfo, string IpAddress);
