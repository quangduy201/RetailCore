using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.Services.Interfaces;

public interface ICategoryService
{
    Task<PagedResult<CategorySummaryDto>> GetPagedAsync(GetCategoriesRequest request);
    Task<PagedResult<CategoryDetailDto>> GetPagedForManagementAsync(GetCategoriesRequest request);
    Task<CategoryDetailDto> GetByIdAsync(Guid id);
    Task<CategoryDetailDto> GetBySlugAsync(string slug);
    Task<Guid> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task DeleteAsync(Guid id);
}
