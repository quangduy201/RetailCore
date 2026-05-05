namespace RetailCore.Shared.Requests.Product;

public class CreateProductAttributeRequest
{
    public string Name { get; set; } = default!;
    public List<CreateProductAttributeValueRequest> Values { get; set; } = [];
}
