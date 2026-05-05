using RetailCore.Shared.Common;
using RetailCore.Shared.Enums;

namespace RetailCore.Shared.Requests.Category;

public class GetCategoriesRequest : PaginationRequest
{
    public string? Keyword { get; set; }
    public CategoryStatus? Status { get; set; }
}
