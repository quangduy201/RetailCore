namespace RetailCore.Shared.DTOs.Category;

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
}
