using RetailCore.Shared.Common;
using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Product;

public class GetProductsRequest : PaginationRequest
{
    public string? Keyword { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? CategoryId { get; set; }
    public ProductStatus? Status { get; set; }
}
