namespace ECommerce.Domain.Enums;

public static class Roles
{
    public const string Customer = "Customer";
    public const string Staff = "Staff";
    public const string Admin = "Admin";

    /// <summary>Roles allowed to operate the admin/staff back office.</summary>
    public const string StaffOrAdmin = "Staff,Admin";
}
