namespace ECommerce.Domain.Enums;

public static class PaymentStatus
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string Expired = "Expired";
}

public static class PaymentMethod
{
    public const string Cod = "COD";
    public const string VnPay = "VNPay";
    public const string MoMo = "MoMo";
}
