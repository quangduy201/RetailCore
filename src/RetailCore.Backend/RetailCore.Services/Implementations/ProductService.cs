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
    private readonly IProductVariantRepository _variantRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepo,
        IProductVariantRepository variantRepo,
        IUnitOfWork unitOfWork)
    {
        _productRepo = productRepo;
        _variantRepo = variantRepo;
        _unitOfWork = unitOfWork;
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

        return new PagedResult<ProductSummaryDto>
        {
            Items = items.Select(MapToProductSummaryDto),
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

        return new PagedResult<ProductManagementDto>
        {
            Items = items.Select(MapToProductManagementDto),
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
            Name = request.Name,
            Slug = request.Slug,
            ShortDescription = request.ShortDescription,
            Description = request.Description,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Status = ProductStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            Attributes = request.Attributes.Select(MapToProductAttribute).ToList(),
        };

        await _productRepo.AddAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return product.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepo.GetTrackedByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Product id '{id}' not found.");

        if (product.Slug != request.Slug && !await _productRepo.IsSlugUniqueAsync(request.Slug, id))
            throw new InvalidOperationException($"Product slug '{request.Slug}' already exists.");

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.ShortDescription = request.ShortDescription;
        product.Description = request.Description;
        product.BrandId = request.BrandId;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        product.Attributes.Clear();
        product.Attributes = request.Attributes.Select(MapToProductAttribute).ToList();

        if (product.Variants.Any())
        {
            _variantRepo.DeleteRange(product.Variants.ToList());
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product id '{id}' not found.");

        _productRepo.Delete(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public static ProductAttribute MapToProductAttribute(ProductAttributeRequest request)
    {
        return new ProductAttribute
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,

            Values = request.Values.Select(v => new ProductAttributeValue
            {
                Id = v.Id ?? Guid.NewGuid(),
                Value = v.Value
            }).ToList()
        };
    }

    public static ProductSummaryDto MapToProductSummaryDto(Product p)
    {
        var firstVariant = p.Variants.FirstOrDefault();

        return new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BrandName = p.Brand.Name,
            CategoryName = p.Category.Name,

            Price = firstVariant?.Price ?? 0,
            CompareAtPrice = firstVariant?.CompareAtPrice,
            DiscountPercentage = firstVariant?.DiscountPercentage,

            ThumbnailUrl = p.Variants
                .SelectMany(v => v.Images)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefault()?.Url,
            IsOutOfStock = p.Variants.Sum(v => v.Stock) <= 0
        };
    }

    public static ProductManagementDto MapToProductManagementDto(Product p)
    {
        return new ProductManagementDto
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
            VariantCount = p.Variants.Count,
            Stock = p.Variants.Sum(v => v.Stock),
            MinPrice = p.Variants.Min(v => (decimal?)v.Price),
            MaxPrice = p.Variants.Max(v => (decimal?)v.Price),
            ThumbnailUrl = p.Variants
                .SelectMany(v => v.Images)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefault()?.Url,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
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
