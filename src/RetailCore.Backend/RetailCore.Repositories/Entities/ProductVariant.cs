using RetailCore.Repositories.Common;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Entities;

public class ProductVariant : Entity, IAuditable
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int Stock { get; set; }
    public ProductVariantStatus Status { get; set; } = ProductVariantStatus.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Product Product { get; set; } = default!;

    public ICollection<ProductVariantImage> Images { get; set; } = [];
    public ICollection<ProductVariantAttribute> Attributes { get; set; } = [];

    // Computed
    public decimal? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > Price
        ? Math.Round(100 * (CompareAtPrice.Value - Price) / CompareAtPrice.Value, 2)
        : null;
}
