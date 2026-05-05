using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.Services.Implementations;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;

    public BrandService(IBrandRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<BrandSummaryDto>> GetPagedAsync(GetBrandsRequest request)
    {
        var (items, totalCount) = await _repo.GetPagedAsync(
            request.Keyword,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(b => new BrandSummaryDto
        {
            Id = b.Id,
            Name = b.Name,
            Slug = b.Slug
        });

        return new PagedResult<BrandSummaryDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<BrandDetailDto>> GetPagedForManagementAsync(GetBrandsRequest request)
    {
        var (items, totalCount) = await _repo.GetPagedAsync(
            request.Keyword,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(MapToBrandDetailDto);

        return new PagedResult<BrandDetailDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BrandDetailDto> GetByIdAsync(Guid id)
    {
        var brand = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Brand id '{id}' not found.");

        return MapToBrandDetailDto(brand);
    }

    public async Task<BrandDetailDto> GetBySlugAsync(string slug)
    {
        var brand = await _repo.GetBySlugAsync(slug)
            ?? throw new KeyNotFoundException($"Brand slug '{slug}' not found.");

        return MapToBrandDetailDto(brand);
    }

    public async Task<Guid> CreateAsync(CreateBrandRequest request)
    {
        if (!await _repo.IsSlugUniqueAsync(request.Slug))
            throw new InvalidOperationException($"Brand slug '{request.Slug}' already exists.");

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Status = BrandStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(brand);

        return brand.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateBrandRequest request)
    {
        var brand = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Brand id '{id}' not found.");

        if (brand.Slug != request.Slug && !await _repo.IsSlugUniqueAsync(request.Slug, id))
            throw new InvalidOperationException($"Brand slug '{request.Slug}' already exists.");

        brand.Name = request.Name;
        brand.Slug = request.Slug;
        brand.Description = request.Description;
        brand.Status = request.Status;
        brand.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(brand);
    }

    public async Task UpdateBrandStatusAsync(Guid id, BrandStatus status)
    {
        var brand = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Brand id '{id}' not found.");

        brand.Status = status;
        brand.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(brand);
    }

    public async Task DeleteAsync(Guid id)
    {
        var brand = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Brand id '{id}' not found.");

        await _repo.DeleteAsync(brand);
    }

    public static BrandDetailDto MapToBrandDetailDto(Brand b)
    {
        return new BrandDetailDto
        {
            Id = b.Id,
            Name = b.Name,
            Slug = b.Slug,
            Description = b.Description,
            Status = b.Status,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
        };
    }
}
