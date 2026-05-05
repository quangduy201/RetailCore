using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.Services.Interfaces;

public interface IBrandService
{
    Task<PagedResult<BrandSummaryDto>> GetPagedAsync(GetBrandsRequest request);
    Task<PagedResult<BrandDetailDto>> GetPagedForManagementAsync(GetBrandsRequest request);
    Task<BrandDetailDto> GetByIdAsync(Guid id);
    Task<BrandDetailDto> GetBySlugAsync(string slug);
    Task<Guid> CreateAsync(CreateBrandRequest request);
    Task UpdateAsync(Guid id, UpdateBrandRequest request);
    Task DeleteAsync(Guid id);
}
