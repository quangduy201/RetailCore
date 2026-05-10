namespace RetailCore.Shared.Requests.Product;

public class ProductAttributeRequest
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = default!;
    public List<ProductAttributeValueRequest> Values { get; set; } = [];
}
