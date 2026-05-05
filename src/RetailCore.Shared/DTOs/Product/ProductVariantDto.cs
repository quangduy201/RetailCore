using RetailCore.Shared.Enums;

namespace RetailCore.Shared.DTOs.Product;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int Stock { get; set; }
    public ProductVariantStatus Status { get; set; }

    public List<ProductVariantImageDto> Images { get; set; } = [];
    public List<ProductVariantAttributeDto> Attributes { get; set; } = [];
}
