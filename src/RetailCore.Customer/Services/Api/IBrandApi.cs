using Refit;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Brand;

namespace RetailCore.Customer.Services.Api;

public interface IBrandApi
{
    [Get("/brands")]
    Task<PagedResult<BrandSummaryDto>> GetBrandsAsync(
        [Query] int pageNumber = 1,
        [Query] int pageSize = 10);
}
