using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByIdAsync(Guid id);
    Task<List<ProductAttribute>> GetByProductIdAsync(Guid productId);
    Task<ProductAttribute?> GetByProductAndNameAsync(Guid productId, string name);
    Task<bool> IsAttributeNameUniqueAsync(Guid productId, string name, Guid? excludeId = null);
    Task AddAsync(ProductAttribute attribute);
    Task UpdateAsync(ProductAttribute attribute);
    Task DeleteAsync(ProductAttribute attribute);
}
