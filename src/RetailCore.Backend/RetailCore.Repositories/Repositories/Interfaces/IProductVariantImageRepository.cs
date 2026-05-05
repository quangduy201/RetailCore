using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductVariantImageRepository
{
    Task<ProductVariantImage?> GetByIdAsync(Guid id);
    Task<List<ProductVariantImage>> GetByVariantIdAsync(Guid variantId);
    Task<ProductVariantImage?> GetPrimaryImageByVariantIdAsync(Guid variantId);
    Task<bool> IsSortOrderUniqueAsync(Guid variantId, int sortOrder, Guid? excludeId = null);
    Task AddAsync(ProductVariantImage image);
    Task UpdateAsync(ProductVariantImage image);
    Task DeleteAsync(ProductVariantImage image);
}
