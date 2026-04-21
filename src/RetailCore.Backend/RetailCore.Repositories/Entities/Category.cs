using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class Category : Entity, IAuditable
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
