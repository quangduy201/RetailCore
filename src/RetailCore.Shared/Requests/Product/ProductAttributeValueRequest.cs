namespace RetailCore.Shared.Requests.Product;

public class ProductAttributeValueRequest
{
    public Guid? Id { get; set; }
    public string Value { get; set; } = default!;
}
