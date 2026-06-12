namespace ECommerce.Infrastructure.Payments;

/// <summary>Bound from configuration section "Vnpay".</summary>
public class VnpaySettings
{
    public const string SectionName = "Vnpay";

    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string IpnUrl { get; set; } = string.Empty;
}
