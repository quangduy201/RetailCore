using Refit;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Customer.Services.Api;

public interface IProductApi
{
    [Get("/products")]
    Task<PagedResult<ProductSummaryDto>> GetProductsAsync(
        [Query] int pageNumber = 1,
        [Query] int pageSize = 9,
        [Query] string? keyword = null,
        [AliasAs("brand")] string? brandSlug = null,
        [AliasAs("category")] string? categorySlug = null);

    [Get("/products/slug/{slug}")]
    Task<ProductDetailDto> GetBySlugAsync(string slug);
}
