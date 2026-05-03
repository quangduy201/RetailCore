using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Shared.Enums;

namespace RetailCore.Repositories.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Brand>, int)> GetPagedAsync(
        string? keyword,
        BrandStatus? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Brands.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(b => b.Name.Contains(keyword) || b.Slug.Contains(keyword));
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Brand?> GetByIdAsync(Guid id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<Brand?> GetBySlugAsync(string slug)
    {
        return await _context.Brands.FirstOrDefaultAsync(b => b.Slug == slug);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null)
    {
        return !await _context.Brands
            .Where(b => b.Slug == slug && (excludeId == null || b.Id != excludeId))
            .AnyAsync();
    }

    public async Task AddAsync(Brand brand)
    {
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Brand brand)
    {
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
    }
}
