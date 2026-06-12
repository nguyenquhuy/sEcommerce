using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Payments;

/// <summary>
/// VNPay sandbox integration. Implements the official param-sorting + HMAC-SHA512 signing
/// scheme so both the redirect URL and the IPN callback can be verified (BR-13 idempotency
/// is handled by the caller using vnp_TxnRef).
/// </summary>
public class VnpayService : IVnpayService
{
    private const string Version = "2.1.0";
    private readonly VnpaySettings _settings;

    public VnpayService(IOptions<VnpaySettings> settings) => _settings = settings.Value;

    public string CreatePaymentUrl(VnpayCreateRequest request)
    {
        // VNPay expects amount in the smallest unit (×100) and a yyyyMMddHHmmss create date.
        var now = DateTime.UtcNow.AddHours(7); // Vietnam time
        var data = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = Version,
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _settings.TmnCode,
            ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(CultureInfo.InvariantCulture),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = request.TxnRef,
            ["vnp_OrderInfo"] = request.OrderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = _settings.ReturnUrl,
            ["vnp_IpAddr"] = string.IsNullOrWhiteSpace(request.IpAddress) ? "127.0.0.1" : request.IpAddress,
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        };

        var signData = BuildSignData(data);
        var secureHash = HmacSha512(_settings.HashSecret, signData);
        return $"{_settings.BaseUrl}?{signData}&vnp_SecureHash={secureHash}";
    }

    public bool ValidateSignature(IDictionary<string, string> queryParams)
    {
        if (!queryParams.TryGetValue("vnp_SecureHash", out var receivedHash) || string.IsNullOrEmpty(receivedHash))
            return false;

        var data = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in queryParams)
        {
            if (key is "vnp_SecureHash" or "vnp_SecureHashType") continue;
            if (key.StartsWith("vnp_", StringComparison.Ordinal))
                data[key] = value;
        }

        var signData = BuildSignData(data);
        var computed = HmacSha512(_settings.HashSecret, signData);
        return string.Equals(computed, receivedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildSignData(SortedDictionary<string, string> data)
        => string.Join("&", data
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

    private static string HmacSha512(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
