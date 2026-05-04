using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Implementations;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _variantRepo;
    private readonly IProductVariantImageRepository _imageRepo;
    private readonly IProductRepository _productRepo;

    public ProductVariantService(
        IProductVariantRepository variantRepo,
        IProductVariantImageRepository imageRepo,
        IProductRepository productRepo)
    {
        _variantRepo = variantRepo;
        _imageRepo = imageRepo;
        _productRepo = productRepo;
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

        if (await ValidateVariantCombinationAsync(productId, request.AttributeValueIds))
            throw new InvalidOperationException("A variant with this attribute combination already exists for this product.");

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            Stock = request.Stock,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _variantRepo.AddAsync(variant);

        foreach (var valId in request.AttributeValueIds)
        {
            variant.Attributes.Add(new ProductVariantAttribute
            {
                Id = Guid.NewGuid(),
                ProductAttributeValueId = valId
            });
        }

        await _variantRepo.AddAsync(variant);

        foreach (var img in request.Images)
        {
            await _imageRepo.AddAsync(new ProductVariantImage
            {
                Id = Guid.NewGuid(),
                ProductVariantId = variant.Id,
                Url = img.Url,
                SortOrder = img.SortOrder,
                IsPrimary = img.IsPrimary,
                CreatedAt = DateTime.UtcNow
            });
        }

        return variant.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateProductVariantRequest request)
    {
        var variant = await _variantRepo.GetByIdAsync(id)
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

        await _variantRepo.UpdateAsync(variant);

        var existing = await _imageRepo.GetByVariantIdAsync(id);

        var toDelete = existing.Where(e => !request.Images.Any(r => r.Id == e.Id));
        foreach (var d in toDelete)
            await _imageRepo.DeleteAsync(d);

        foreach (var r in request.Images)
        {
            if (r.Id == null)
            {
                await _imageRepo.AddAsync(new ProductVariantImage
                {
                    Id = Guid.NewGuid(),
                    ProductVariantId = id,
                    Url = r.Url,
                    SortOrder = r.SortOrder,
                    IsPrimary = r.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                var img = existing.First(x => x.Id == r.Id);
                img.Url = r.Url;
                img.SortOrder = r.SortOrder;
                img.IsPrimary = r.IsPrimary;
                img.UpdatedAt = DateTime.UtcNow;

                await _imageRepo.UpdateAsync(img);
            }
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var variant = await _variantRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Variant id '{id}' not found.");

        await _variantRepo.DeleteAsync(variant);
    }

    public async Task<bool> ValidateVariantCombinationAsync(Guid productId, List<Guid> attributeValueIds)
    {
        var variants = await _variantRepo.GetByProductIdAsync(productId);

        var exists = variants.Any(v =>
            v.Attributes.Select(a => a.ProductAttributeValueId)
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
            Status = variant.Status
        };
    }
}
