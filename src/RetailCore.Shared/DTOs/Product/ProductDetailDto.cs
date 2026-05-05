using RetailCore.Shared.Enums;

namespace RetailCore.Shared.DTOs.Product;

public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public Guid BrandId { get; set; }
    public string BrandName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;

    public ProductStatus Status { get; set; }

    public List<ProductAttributeDto> Attributes { get; set; } = [];
    public List<ProductVariantDto> Variants { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
