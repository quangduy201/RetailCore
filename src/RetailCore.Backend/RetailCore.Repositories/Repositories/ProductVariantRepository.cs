using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;

namespace RetailCore.Repositories.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly AppDbContext _context;

    public ProductVariantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id)
    {
        return await BuildQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<ProductVariant?> GetTrackedByIdAsync(Guid id)
    {
        return await BuildQuery()
            .FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<List<ProductVariant>> GetByProductIdAsync(Guid productId)
    {
        return await BuildQuery()
            .AsNoTracking()
            .Where(pv => pv.ProductId == productId)
            .ToListAsync();
    }

    public async Task<List<ProductVariant>> GetTrackedByProductIdAsync(Guid productId)
    {
        return await BuildQuery()
            .Where(pv => pv.ProductId == productId)
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetBySkuAsync(string sku)
    {
        return await BuildQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(pv => pv.Sku == sku);
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeId = null)
    {
        return !await _context.ProductVariants
            .AnyAsync(v => v.Sku == sku && (!excludeId.HasValue || v.Id != excludeId.Value));
    }

    public async Task AddAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
    }

    public void Update(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
    }

    public void Delete(ProductVariant variant)
    {
        _context.ProductVariants.Remove(variant);
    }

    public void DeleteRange(List<ProductVariant> variants)
    {
        _context.ProductVariants.RemoveRange(variants);
    }

    public void RemoveImages(IEnumerable<ProductVariantImage> images)
    {
        _context.ProductVariantImages.RemoveRange(images);
    }

    public void RemoveAttributes(IEnumerable<ProductVariantAttribute> attributes)
    {
        _context.ProductVariantAttributes.RemoveRange(attributes);
    }

    private IQueryable<ProductVariant> BuildQuery()
    {
        return _context.ProductVariants
            .Include(v => v.Images)
            .Include(v => v.Attributes)
                .ThenInclude(a => a.ProductAttributeValue)
                    .ThenInclude(v => v.ProductAttribute);
    }
}
