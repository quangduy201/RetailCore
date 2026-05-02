using RetailCore.Shared.Common;
using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Brand;

public class GetBrandsRequest : PaginationRequest
{
    public string? Keyword { get; set; }
    public BrandStatus? Status { get; set; }
}
