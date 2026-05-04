namespace RetailCore.Shared.Requests.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }

    public List<CreateProductAttributeRequest> Attributes { get; set; } = [];
    public List<CreateProductVariantRequest> Variants { get; set; } = [];
}
