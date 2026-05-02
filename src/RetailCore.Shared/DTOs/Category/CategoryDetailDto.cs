using RetailCore.Shared.Enums;

namespace RetailCore.Shared.DTOs.Category;

public class CategoryDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public CategoryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
