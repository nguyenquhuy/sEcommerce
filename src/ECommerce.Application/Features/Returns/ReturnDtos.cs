namespace ECommerce.Application.Features.Returns;

public class ReturnRequestDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? StaffNote { get; set; }
    public decimal RefundAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<ReturnItemDto> Items { get; set; } = Array.Empty<ReturnItemDto>();
}

public class ReturnItemDto
{
    public Guid OrderItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>Input line for a return request.</summary>
public record ReturnItemInput(Guid OrderItemId, int Quantity);
