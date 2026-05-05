
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Interfaces;

public interface IProductAttributeService
{
    Task<List<ProductAttributeDto>> GetByProductIdAsync(Guid productId);
    Task<ProductAttributeDto> GetByIdAsync(Guid attributeId);
    Task<Guid> CreateAsync(Guid productId, CreateProductAttributeRequest request);
    Task UpdateAsync(Guid attributeId, UpdateProductAttributeRequest request);
    Task DeleteAsync(Guid attributeId);

    Task<List<ProductAttributeValueDto>> GetValuesByAttributeIdAsync(Guid attributeId);
    Task<ProductAttributeValueDto> GetValueByIdAsync(Guid valueId);
    Task<Guid> CreateValueAsync(Guid attributeId, CreateProductAttributeValueRequest request);
    Task UpdateValueAsync(Guid valueId, UpdateProductAttributeValueRequest request);
    Task DeleteValueAsync(Guid valueId);
}
