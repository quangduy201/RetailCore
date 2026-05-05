namespace RetailCore.Shared.Requests.Brand;

public class CreateBrandRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
}
