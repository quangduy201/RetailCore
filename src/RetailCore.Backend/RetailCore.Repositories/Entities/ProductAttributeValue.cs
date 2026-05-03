using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class ProductAttributeValue : Entity
{
    public Guid ProductAttributeId { get; set; }
    public string Value { get; set; } = default!;

    // Navigation
    public ProductAttribute ProductAttribute { get; set; } = default!;

    public ICollection<ProductVariantAttribute> VariantAttributes { get; set; } = [];
}
