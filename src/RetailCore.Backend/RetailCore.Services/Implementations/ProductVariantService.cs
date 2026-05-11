using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Implementations;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductRepository _productRepo;
    private readonly IProductVariantRepository _variantRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ProductVariantService(
        IProductRepository productRepo,
        IProductVariantRepository variantRepo,
        IUnitOfWork unitOfWork)
    {
        _productRepo = productRepo;
        _variantRepo = variantRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductVariantDto> GetByIdAsync(Guid id)
    {
        var variant = await _variantRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Variant id '{id}' not found.");

        return MapToProductVariantDto(variant);
    }

    public async Task<List<ProductVariantDto>> GetByProductIdAsync(Guid productId)
    {
        var variants = await _variantRepo.GetByProductIdAsync(productId);

        return variants.Select(MapToProductVariantDto).ToList();
    }

    public async Task<Guid> CreateAsync(Guid productId, CreateProductVariantRequest request)
    {
        _ = await _productRepo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

        if (!await _variantRepo.IsSkuUniqueAsync(request.Sku))
            throw new InvalidOperationException($"SKU '{request.Sku}' already exists.");

        if (!await IsCombinationUniqueAsync(productId, request.AttributeValueIds))
            throw new InvalidOperationException("Variant combination already exists.");

        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            Stock = request.Stock,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            Images = request.Images.Select(i => new ProductVariantImage
            {
                Url = i.Url,
                SortOrder = i.SortOrder,
                IsPrimary = i.IsPrimary,
                CreatedAt = DateTime.UtcNow
            }).ToList(),
            Attributes = request.AttributeValueIds.Select(v => new ProductVariantAttribute
            {
                ProductAttributeValueId = v
            }).ToList()
        };

        await _variantRepo.AddAsync(variant);

        await _unitOfWork.SaveChangesAsync();

        return variant.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateProductVariantRequest request)
    {
        var variant = await _variantRepo.GetTrackedByIdAsync(id)
            ?? throw new KeyNotFoundException($"Variant id '{id}' not found.");

        if (variant.Sku != request.Sku && !await _variantRepo.IsSkuUniqueAsync(request.Sku, id))
            throw new InvalidOperationException($"SKU '{request.Sku}' already exists.");

        variant.Sku = request.Sku;
        variant.Name = request.Name;
        variant.Description = request.Description;
        variant.Price = request.Price;
        variant.CompareAtPrice = request.CompareAtPrice;
        variant.Stock = request.Stock;
        variant.Status = request.Status;
        variant.UpdatedAt = DateTime.UtcNow;

        if (request.Images != null)
        {
            // Remove old images
            _variantRepo.RemoveImages(variant.Images);

            // Recreate images
            variant.Images = request.Images
                .Select(image => new ProductVariantImage
                {
                    ProductVariantId = variant.Id,
                    Url = image.Url,
                    SortOrder = image.SortOrder,
                    IsPrimary = image.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();
        }

        // Remove old attributes
        _variantRepo.RemoveAttributes(variant.Attributes);

        // Recreate attributes
        variant.Attributes = request.AttributeValueIds
            .Select(attributeValueId => new ProductVariantAttribute
            {
                ProductVariantId = variant.Id,
                ProductAttributeValueId = attributeValueId
            })
            .ToList();

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteDraftAsync(Guid id)
    {
        var variant = await _variantRepo.GetTrackedByIdAsync(id)
            ?? throw new KeyNotFoundException($"Variant id '{id}' not found.");

        _variantRepo.Delete(variant);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveAllVariantsAsync(Guid productId)
    {
        var variants = await _variantRepo.GetTrackedByProductIdAsync(productId);

        _variantRepo.DeleteRange(variants);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsCombinationUniqueAsync(Guid productId, List<Guid> attributeValueIds)
    {
        var variants = await _variantRepo.GetByProductIdAsync(productId);

        var exists = variants.Any(v =>
            v.Attributes
                .Select(a => a.ProductAttributeValueId)
                .OrderBy(x => x)
                .SequenceEqual(attributeValueIds.OrderBy(x => x)));

        return !exists;
    }

    public static ProductVariantDto MapToProductVariantDto(ProductVariant variant)
    {
        return new ProductVariantDto
        {
            Id = variant.Id,
            Sku = variant.Sku,
            Name = variant.Name,
            Description = variant.Description,
            Price = variant.Price,
            CompareAtPrice = variant.CompareAtPrice,
            DiscountPercentage = variant.DiscountPercentage,
            Stock = variant.Stock,
            Status = variant.Status,
            Images = variant.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => new ProductVariantImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary
                }).ToList(),

            Attributes = variant.Attributes
                .Select(a => new ProductVariantAttributeDto
                {
                    AttributeId = a.ProductAttributeValue.ProductAttributeId,
                    AttributeName = a.ProductAttributeValue.ProductAttribute.Name,

                    AttributeValueId = a.ProductAttributeValueId,
                    AttributeValue = a.ProductAttributeValue.Value
                }).ToList()
        };
    }
}
