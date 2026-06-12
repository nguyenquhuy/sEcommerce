namespace ECommerce.Application.Features.Addresses;

public class AddressDto
{
    public Guid Id { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string Street { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
