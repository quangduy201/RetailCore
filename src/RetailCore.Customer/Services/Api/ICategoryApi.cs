using Refit;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Category;

namespace RetailCore.Customer.Services.Api;

public interface ICategoryApi
{
    [Get("/categories")]
    Task<PagedResult<CategorySummaryDto>> GetCategoriesAsync(
        [Query] int pageNumber = 1,
        [Query] int pageSize = 10);
}
