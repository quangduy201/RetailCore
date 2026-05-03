using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class ProductVariantImage : Entity, IAuditable
{
    public Guid ProductVariantId { get; set; }

    public string Url { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ProductVariant ProductVariant { get; set; } = default!;
}
