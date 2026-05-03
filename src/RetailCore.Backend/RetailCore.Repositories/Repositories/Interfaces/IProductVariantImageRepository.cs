using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductVariantImageRepository
{
    Task<List<ProductVariantImage>> GetAllAsync();
    Task<ProductVariantImage?> GetByIdAsync(Guid id);
    Task<List<ProductVariantImage>> GetByVariantIdAsync(Guid variantId);
    Task<ProductVariantImage?> GetPrimaryImageByVariantIdAsync(Guid variantId);
    Task<bool> IsSortOrderUniqueAsync(Guid variantId, int sortOrder, Guid? excludeId = null);
    Task AddAsync(ProductVariantImage image);
    void Update(ProductVariantImage image);
    void Delete(ProductVariantImage image);
    Task SaveChangesAsync();
}
