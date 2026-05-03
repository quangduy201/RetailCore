using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Repositories.Interfaces;

public interface IProductAttributeValueRepository
{
    Task<ProductAttributeValue?> GetByIdAsync(Guid id);
    Task<List<ProductAttributeValue>> GetByAttributeIdAsync(Guid attributeId);
    Task<ProductAttributeValue?> GetByAttributeAndValueAsync(Guid attributeId, string value);
    Task<bool> IsValueUniqueAsync(Guid attributeId, string value, Guid? excludeId = null);
    Task AddAsync(ProductAttributeValue value);
    Task UpdateAsync(ProductAttributeValue value);
    Task DeleteAsync(ProductAttributeValue value);
}
