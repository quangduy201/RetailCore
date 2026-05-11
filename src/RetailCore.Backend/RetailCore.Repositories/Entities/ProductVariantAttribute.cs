namespace RetailCore.Repositories.Entities;

public class ProductVariantAttribute
{
    public Guid ProductVariantId { get; set; }
    public Guid ProductAttributeValueId { get; set; }

    // Navigation
    public ProductVariant ProductVariant { get; set; } = default!;
    public ProductAttributeValue ProductAttributeValue { get; set; } = default!;
}
