using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;

namespace RetailCore.Repositories.Repositories;

public class ProductVariantImageRepository : IProductVariantImageRepository
{
    private readonly AppDbContext _context;

    public ProductVariantImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariantImage?> GetByIdAsync(Guid id)
    {
        return await _context.ProductVariantImages.FindAsync(id);
    }

    public async Task<List<ProductVariantImage>> GetByVariantIdAsync(Guid variantId)
    {
        return await _context.ProductVariantImages
            .Where(pvi => pvi.ProductVariantId == variantId)
            .OrderBy(pvi => pvi.SortOrder)
            .ToListAsync();
    }

    public async Task<ProductVariantImage?> GetPrimaryImageByVariantIdAsync(Guid variantId)
    {
        return await _context.ProductVariantImages
            .FirstOrDefaultAsync(pvi => pvi.ProductVariantId == variantId && pvi.IsPrimary);
    }

    public async Task<bool> IsSortOrderUniqueAsync(Guid variantId, int sortOrder, Guid? excludeId = null)
    {
        return !await _context.ProductVariantImages
            .Where(pvi => pvi.ProductVariantId == variantId && pvi.SortOrder == sortOrder && (excludeId == null || pvi.Id != excludeId))
            .AnyAsync();
    }

    public async Task AddAsync(ProductVariantImage image)
    {
        await _context.ProductVariantImages.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductVariantImage image)
    {
        _context.ProductVariantImages.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductVariantImage image)
    {
        _context.ProductVariantImages.Remove(image);
        await _context.SaveChangesAsync();
    }
}
