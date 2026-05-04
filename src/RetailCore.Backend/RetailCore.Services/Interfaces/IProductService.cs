using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductSummaryDto>> GetPagedAsync(GetProductsRequest request);
    Task<PagedResult<ProductManagementDto>> GetPagedForManagementAsync(GetProductsRequest request);
    Task<ProductDetailDto> GetByIdAsync(Guid id);
    Task<ProductDetailDto> GetBySlugAsync(string slug);
    Task<Guid> CreateAsync(CreateProductRequest request);
    Task UpdateAsync(Guid id, UpdateProductRequest request);
    Task DeleteAsync(Guid id);
}
