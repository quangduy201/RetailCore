
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Interfaces;

public interface IProductAttributeService
{
    Task<List<ProductAttributeDto>> GetByProductIdAsync(Guid productId);
    Task<ProductAttributeDto> GetByIdAsync(Guid attributeId);
    Task<Guid> CreateAsync(Guid productId, ProductAttributeRequest request);
    Task UpdateAsync(Guid attributeId, ProductAttributeRequest request);
    Task DeleteAsync(Guid attributeId);

    Task<List<ProductAttributeValueDto>> GetValuesByAttributeIdAsync(Guid attributeId);
    Task<ProductAttributeValueDto> GetValueByIdAsync(Guid valueId);
    Task<Guid> CreateValueAsync(Guid attributeId, ProductAttributeValueRequest request);
    Task UpdateValueAsync(Guid valueId, ProductAttributeValueRequest request);
    Task DeleteValueAsync(Guid valueId);
}
