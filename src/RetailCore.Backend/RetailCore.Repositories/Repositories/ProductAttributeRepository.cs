using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;

namespace RetailCore.Repositories.Repositories;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly AppDbContext _context;

    public ProductAttributeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductAttribute?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAttributes
            .Include(pa => pa.Values)
            .FirstOrDefaultAsync(pa => pa.Id == id);
    }

    public async Task<List<ProductAttribute>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.ProductId == productId)
            .Include(pa => pa.Values)
            .ToListAsync();
    }

    public async Task<ProductAttribute?> GetByProductAndNameAsync(Guid productId, string name)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.ProductId == productId && pa.Name == name)
            .Include(pa => pa.Values)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsAttributeNameUniqueAsync(Guid productId, string name, Guid? excludeId = null)
    {
        return !await _context.ProductAttributes
            .Where(pa => pa.ProductId == productId && pa.Name == name && (excludeId == null || pa.Id != excludeId))
            .AnyAsync();
    }

    public async Task AddAsync(ProductAttribute attribute)
    {
        await _context.ProductAttributes.AddAsync(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Remove(attribute);
        await _context.SaveChangesAsync();
    }
}
