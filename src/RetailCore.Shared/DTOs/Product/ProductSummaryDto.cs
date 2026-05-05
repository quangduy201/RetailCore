namespace RetailCore.Shared.DTOs.Product;

public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public string BrandName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;

    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsOutOfStock { get; set; }
}
