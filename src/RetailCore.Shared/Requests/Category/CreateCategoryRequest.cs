namespace RetailCore.Shared.Requests.Category;

public class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
}
