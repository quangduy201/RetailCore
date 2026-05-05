using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Product;

public class CreateProductVariantRequest
{
    public string Sku { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int Stock { get; set; }
    public ProductVariantStatus Status { get; set; }

    public List<Guid> AttributeValueIds { get; set; } = [];
    public List<CreateProductVariantImageRequest> Images { get; set; } = [];
}
