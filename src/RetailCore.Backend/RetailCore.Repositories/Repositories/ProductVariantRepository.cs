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
        return await _context.ProductVariants
            .Include(pv => pv.Images.OrderBy(i => i.SortOrder))
            .Include(pv => pv.Attributes)
                .ThenInclude(va => va.ProductAttributeValue)
                    .ThenInclude(pav => pav.ProductAttribute)
            .FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<List<ProductVariant>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductVariants
            .Where(pv => pv.ProductId == productId)
            .Include(pv => pv.Images.OrderBy(i => i.SortOrder))
            .Include(pv => pv.Attributes)
                .ThenInclude(va => va.ProductAttributeValue)
                    .ThenInclude(pav => pav.ProductAttribute)
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetBySkuAsync(string sku)
    {
        return await _context.ProductVariants
            .FirstOrDefaultAsync(pv => pv.Sku == sku);
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeId = null)
    {
        return !await _context.ProductVariants
            .Where(pv => pv.Sku == sku && (excludeId == null || pv.Id != excludeId))
            .AnyAsync();
    }

    public async Task AddAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductVariant variant)
    {
        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync();
    }
}
