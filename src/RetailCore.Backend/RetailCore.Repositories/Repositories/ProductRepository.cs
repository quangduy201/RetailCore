using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Product>, int)> GetPagedAsync(
        string? keyword,
        Guid? brandId,
        Guid? categoryId,
        ProductStatus? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.Name.Contains(keyword) || p.Slug.Contains(keyword));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId);

        if (status.HasValue)
            query = query.Where(p => p.Status == status);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Product>, int)> GetPagedForManagementAsync(
        string? keyword,
        ProductStatus? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.Name.Contains(keyword) || p.Slug.Contains(keyword));

        if (status.HasValue)
            query = query.Where(p => p.Status == status);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Attributes)
                .ThenInclude(pa => pa.Values)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Attributes)
                    .ThenInclude(va => va.ProductAttributeValue)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Product?> GetBySlugWithDetailsAsync(string slug)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Attributes)
                .ThenInclude(pa => pa.Values)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Attributes)
                    .ThenInclude(va => va.ProductAttributeValue)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null)
    {
        return !await _context.Products
            .AnyAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
