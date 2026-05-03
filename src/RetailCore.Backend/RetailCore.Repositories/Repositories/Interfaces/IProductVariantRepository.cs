using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductVariantRepository
{
    Task<ProductVariant?> GetByIdAsync(Guid id);
    Task<List<ProductVariant>> GetByProductIdAsync(Guid productId);
    Task<ProductVariant?> GetBySkuAsync(string sku);
    Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeId = null);
    Task AddAsync(ProductVariant variant);
    Task UpdateAsync(ProductVariant variant);
    Task DeleteAsync(ProductVariant variant);
}
