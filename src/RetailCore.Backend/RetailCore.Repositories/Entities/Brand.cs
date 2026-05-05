using RetailCore.Repositories.Common;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Entities;

public class Brand : Entity, IAuditable
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public BrandStatus Status { get; set; } = BrandStatus.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}
