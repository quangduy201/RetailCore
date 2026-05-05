namespace RetailCore.Shared.Requests.Product;

public class UpdateProductAttributeValueRequest
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
}
