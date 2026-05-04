using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Interfaces;

public interface IProductVariantService
{
    Task<List<ProductVariantDto>> GetByProductIdAsync(Guid productId);
    Task<ProductVariantDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Guid productId, CreateProductVariantRequest request);
    Task UpdateAsync(Guid id, UpdateProductVariantRequest request);
    Task DeleteAsync(Guid id);
    Task<bool> ValidateVariantCombinationAsync(Guid productId, List<Guid> attributeValueIds);
}
