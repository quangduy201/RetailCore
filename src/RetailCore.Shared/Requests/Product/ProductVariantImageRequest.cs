namespace RetailCore.Shared.Requests.Product;

public class ProductVariantImageRequest
{
    public Guid? Id { get; set; }
    public string Url { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
