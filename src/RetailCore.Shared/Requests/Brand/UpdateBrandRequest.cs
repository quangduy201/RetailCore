using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Brand;

public class UpdateBrandRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public BrandStatus Status { get; set; }
}
