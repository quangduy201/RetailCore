using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class Product : Entity, IAuditable
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Brand Brand { get; set; } = default!;
    public Category Category { get; set; } = default!;

    public ICollection<ProductAttribute> Attributes { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
}

public enum ProductStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    Archived = 3
}
