namespace RetailCore.Shared.DTOs.Product;

public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<ProductAttributeValueDto> Values { get; set; } = [];
}
