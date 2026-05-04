namespace RetailCore.Shared.Requests.Product;

public class CreateProductVariantImageRequest
{
    public string Url { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
