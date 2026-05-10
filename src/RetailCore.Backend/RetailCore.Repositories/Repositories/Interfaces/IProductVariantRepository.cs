using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductVariantRepository
{
    Task<ProductVariant?> GetByIdAsync(Guid id);
    Task<ProductVariant?> GetTrackedByIdAsync(Guid id);
    Task<List<ProductVariant>> GetByProductIdAsync(Guid productId);
    Task<List<ProductVariant>> GetTrackedByProductIdAsync(Guid productId);
    Task<ProductVariant?> GetBySkuAsync(string sku);
    Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeId = null);
    Task AddAsync(ProductVariant variant);
    void Update(ProductVariant variant);
    void Delete(ProductVariant variant);
    void DeleteRange(List<ProductVariant> variants);
}
