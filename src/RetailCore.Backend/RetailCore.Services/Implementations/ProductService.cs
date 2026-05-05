using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IProductAttributeRepository _attributeRepo;
    private readonly IProductVariantService _variantService;

    public ProductService(
        IProductRepository productRepo,
        IProductAttributeRepository attributeRepo,
        IProductVariantService variantService)
    {
        _productRepo = productRepo;
        _attributeRepo = attributeRepo;
        _variantService = variantService;
    }

    public async Task<PagedResult<ProductSummaryDto>> GetPagedAsync(GetProductsRequest request)
    {
        var (items, totalCount) = await _productRepo.GetPagedAsync(
            request.Keyword,
            request.BrandId,
            request.CategoryId,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BrandName = p.Brand.Name,
            CategoryName = p.Category.Name,
            Price = p.Variants.FirstOrDefault()?.Price ?? 0,
            CompareAtPrice = p.Variants.FirstOrDefault()?.CompareAtPrice,
            DiscountPercentage = p.Variants.FirstOrDefault()?.DiscountPercentage,
            ThumbnailUrl = p.Variants
                .SelectMany(v => v.Images)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.Url)
                .FirstOrDefault(),
            IsOutOfStock = p.Variants.Sum(v => v.Stock) == 0
        });

        return new PagedResult<ProductSummaryDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<ProductManagementDto>> GetPagedForManagementAsync(GetProductsRequest request)
    {
        var (items, totalCount) = await _productRepo.GetPagedForManagementAsync(
            request.Keyword,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(p => new ProductManagementDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BrandId = p.BrandId,
            BrandName = p.Brand.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Status = p.Status,
            VariantCount = p.Variants.Count,
            Stock = p.Variants.Sum(v => v.Stock),
            MinPrice = p.Variants.Min(v => (decimal?)v.Price),
            MaxPrice = p.Variants.Max(v => (decimal?)v.Price),
            ThumbnailUrl = p.Variants
                .SelectMany(v => v.Images)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.Url)
                .FirstOrDefault(),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return new PagedResult<ProductManagementDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDetailDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Product id '{id}' not found.");

        return MapToProductDetailDto(product);
    }

    public async Task<ProductDetailDto> GetBySlugAsync(string slug)
    {
        var product = await _productRepo.GetBySlugWithDetailsAsync(slug)
            ?? throw new KeyNotFoundException($"Product slug '{slug}' not found.");

        return MapToProductDetailDto(product);
    }

    public async Task<Guid> CreateAsync(CreateProductRequest request)
    {
        if (!await _productRepo.IsSlugUniqueAsync(request.Slug))
            throw new InvalidOperationException($"Product slug '{request.Slug}' already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            ShortDescription = request.ShortDescription,
            Description = request.Description,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Status = ProductStatus.Draft,
            CreatedAt = DateTime.UtcNow,

            // Variants = request.Variants.Select(v => new ProductVariant
            // {
            //     Id = Guid.NewGuid(),
            //     Sku = v.Sku,
            //     Price = v.Price,
            //     Stock = v.Stock,
            //     Images = v.Images.Select(i => new ProductVariantImage
            //     {
            //         Id = Guid.NewGuid(),
            //         Url = i.Url,
            //         SortOrder = i.SortOrder,
            //         IsPrimary = i.IsPrimary
            //     }).ToList(),

            //     Attributes = v.Attributes.Select(a => new ProductVariantAttribute
            //     {
            //         Id = Guid.NewGuid(),
            //         ProductAttributeValueId = a.
            //     }).ToList()
            // }).ToList(),

            // Attributes = request.Attributes.Select(a => new ProductAttribute
            // {
            //     Id = Guid.NewGuid(),
            //     Name = a.Name,
            //     Values = a.Values.Select(v => new ProductAttributeValue
            //     {
            //         Id = Guid.NewGuid(),
            //         Value = v.
            //     }).ToList()
            // })
        };

        await _productRepo.AddAsync(product);

        // Attributes
        foreach (var attr in request.Attributes)
        {
            var attribute = new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = attr.Name
            };

            await _attributeRepo.AddAsync(attribute);
        }

        // Variants
        foreach (var variant in request.Variants)
        {
            await _variantService.CreateAsync(product.Id, variant);
        }

        return product.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product id '{id}' not found.");

        if (product.Slug != request.Slug && !await _productRepo.IsSlugUniqueAsync(request.Slug, id))
            throw new InvalidOperationException($"Product slug '{request.Slug}' already exists.");

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.ShortDescription = request.ShortDescription;
        product.Description = request.Description;
        product.BrandId = request.BrandId;
        product.CategoryId = request.CategoryId;
        product.Status = request.Status;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepo.UpdateAsync(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product id '{id}' not found.");

        await _productRepo.DeleteAsync(product);
    }

    public static ProductDetailDto MapToProductDetailDto(Product p)
    {
        return new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            BrandId = p.BrandId,
            BrandName = p.Brand.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Status = p.Status,
            Attributes = p.Attributes.Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                Name = a.Name,
                Values = a.Values.Select(v => new ProductAttributeValueDto
                {
                    Id = v.Id,
                    Value = v.Value
                }).ToList()
            }).ToList(),
            Variants = p.Variants.Select(ProductVariantService.MapToProductVariantDto).ToList(),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
