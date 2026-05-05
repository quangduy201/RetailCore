namespace RetailCore.Shared.DTOs.Brand;

public class BrandSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
}
