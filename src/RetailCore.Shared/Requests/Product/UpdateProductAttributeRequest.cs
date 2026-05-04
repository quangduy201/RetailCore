namespace RetailCore.Shared.Requests.Product;

public class UpdateProductAttributeRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
