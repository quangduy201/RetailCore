using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class Product : Entity, IAuditable
{
    public string Name { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
