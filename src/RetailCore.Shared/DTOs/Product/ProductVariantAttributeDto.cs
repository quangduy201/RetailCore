namespace RetailCore.Shared.DTOs.Product;

public class ProductVariantAttributeDto
{
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; } = default!;

    public Guid AttributeValueId { get; set; }
    public string AttributeValue { get; set; } = default!;
}
