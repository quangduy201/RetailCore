namespace RetailCore.Shared.DTOs.Category;

public class CategoryUpdateDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
