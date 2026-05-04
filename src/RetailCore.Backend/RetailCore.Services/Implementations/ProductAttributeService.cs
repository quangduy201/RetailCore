using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Implementations;

public class ProductAttributeService : IProductAttributeService
{
    private readonly IProductAttributeRepository _attributeRepo;
    private readonly IProductAttributeValueRepository _valueRepo;
    private readonly IProductRepository _productRepo;

    public ProductAttributeService(
        IProductAttributeRepository attributeRepo,
        IProductAttributeValueRepository valueRepo,
        IProductRepository productRepo)
    {
        _attributeRepo = attributeRepo;
        _valueRepo = valueRepo;
        _productRepo = productRepo;
    }

    public async Task<List<ProductAttributeDto>> GetByProductIdAsync(Guid productId)
    {
        var attributes = await _attributeRepo.GetByProductIdAsync(productId);
        return attributes.Select(MapToProductAttributeDto).ToList();
    }

    public async Task<ProductAttributeDto> GetByIdAsync(Guid attributeId)
    {
        var attribute = await _attributeRepo.GetByIdAsync(attributeId)
            ?? throw new KeyNotFoundException($"Attribute id '{attributeId}' not found.");

        return MapToProductAttributeDto(attribute);
    }

    public async Task<Guid> CreateAsync(Guid productId, CreateProductAttributeRequest request)
    {
        _ = await _productRepo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

        if (!await _attributeRepo.IsAttributeNameUniqueAsync(productId, request.Name))
            throw new InvalidOperationException($"An attribute with name '{request.Name}' already exists for this product.");

        var attribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = request.Name
        };

        await _attributeRepo.AddAsync(attribute);

        return attribute.Id;
    }

    public async Task UpdateAsync(Guid attributeId, UpdateProductAttributeRequest request)
    {
        var attribute = await _attributeRepo.GetByIdAsync(attributeId)
            ?? throw new KeyNotFoundException($"Attribute id '{attributeId}' not found.");

        if (attribute.Name != request.Name && !await _attributeRepo.IsAttributeNameUniqueAsync(attribute.ProductId, request.Name, attributeId))
            throw new InvalidOperationException($"Attribute name '{request.Name}' already exists for this product.");

        attribute.Name = request.Name;

        await _attributeRepo.UpdateAsync(attribute);
    }

    public async Task DeleteAsync(Guid attributeId)
    {
        var attribute = await _attributeRepo.GetByIdAsync(attributeId)
            ?? throw new KeyNotFoundException($"Attribute id '{attributeId}' not found.");

        await _attributeRepo.DeleteAsync(attribute);
    }

    public async Task<List<ProductAttributeValueDto>> GetValuesByAttributeIdAsync(Guid attributeId)
    {
        var values = await _valueRepo.GetByAttributeIdAsync(attributeId);

        return values.Select(MapToProductAttributeValueDto).ToList();
    }

    public async Task<ProductAttributeValueDto> GetValueByIdAsync(Guid valueId)
    {
        var value = await _valueRepo.GetByIdAsync(valueId)
            ?? throw new KeyNotFoundException($"Attribute value id '{valueId}' not found.");

        return MapToProductAttributeValueDto(value);
    }

    public async Task<Guid> CreateValueAsync(Guid attributeId, CreateProductAttributeValueRequest request)
    {
        var value = new ProductAttributeValue
        {
            Id = Guid.NewGuid(),
            ProductAttributeId = attributeId,
            Value = request.Value
        };

        await _valueRepo.AddAsync(value);

        return value.Id;
    }

    public async Task UpdateValueAsync(Guid valueId, UpdateProductAttributeValueRequest request)
    {
        var value = await _valueRepo.GetByIdAsync(valueId)
            ?? throw new KeyNotFoundException($"Attribute value id '{valueId}' not found.");

        value.Value = request.Value;

        await _valueRepo.UpdateAsync(value);
    }

    public async Task DeleteValueAsync(Guid valueId)
    {
        var value = await _valueRepo.GetByIdAsync(valueId)
            ?? throw new KeyNotFoundException($"Attribute value id '{valueId}' not found.");

        await _valueRepo.DeleteAsync(value);
    }

    public static ProductAttributeDto MapToProductAttributeDto(ProductAttribute a)
    {
        return new ProductAttributeDto
        {
            Id = a.Id,
            Name = a.Name,
            Values = a.Values.Select(MapToProductAttributeValueDto).ToList()
        };
    }

    private static ProductAttributeValueDto MapToProductAttributeValueDto(ProductAttributeValue av)
    {
        return new ProductAttributeValueDto
        {
            Id = av.Id,
            Value = av.Value
        };
    }
}
