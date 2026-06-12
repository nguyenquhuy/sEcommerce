namespace ECommerce.Application.Features.Reviews;

/// <summary>Public review shown on a product page.</summary>
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
