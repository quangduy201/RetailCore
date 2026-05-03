using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;

namespace RetailCore.Repositories.Repositories;

public class ProductAttributeValueRepository : IProductAttributeValueRepository
{
    private readonly AppDbContext _context;

    public ProductAttributeValueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductAttributeValue?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAttributeValues.FindAsync(id);
    }

    public async Task<List<ProductAttributeValue>> GetByAttributeIdAsync(Guid attributeId)
    {
        return await _context.ProductAttributeValues
            .Where(pav => pav.ProductAttributeId == attributeId)
            .ToListAsync();
    }

    public async Task<ProductAttributeValue?> GetByAttributeAndValueAsync(Guid attributeId, string value)
    {
        return await _context.ProductAttributeValues
            .FirstOrDefaultAsync(pav => pav.ProductAttributeId == attributeId && pav.Value == value);
    }

    public async Task<bool> IsValueUniqueAsync(Guid attributeId, string value, Guid? excludeId = null)
    {
        return !await _context.ProductAttributeValues
            .Where(pav => pav.ProductAttributeId == attributeId && pav.Value == value && (excludeId == null || pav.Id != excludeId))
            .AnyAsync();
    }

    public async Task AddAsync(ProductAttributeValue value)
    {
        await _context.ProductAttributeValues.AddAsync(value);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductAttributeValue value)
    {
        _context.ProductAttributeValues.Update(value);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductAttributeValue value)
    {
        _context.ProductAttributeValues.Remove(value);
        await _context.SaveChangesAsync();
    }
}
