using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Product;

public class UpdateProductRequest
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }

    public ProductStatus Status { get; set; }
}
