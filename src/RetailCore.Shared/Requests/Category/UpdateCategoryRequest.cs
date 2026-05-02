using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Category;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public CategoryStatus Status { get; set; }
}
