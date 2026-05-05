using RetailCore.Repositories.Common;

namespace RetailCore.Repositories.Entities;

public class ProductAttribute : Entity
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = default!;

    // Navigation
    public Product Product { get; set; } = default!;

    public ICollection<ProductAttributeValue> Values { get; set; } = [];
}
