using RetailCore.Repositories.Entities;
using RetailCore.Repositories.Repositories.Interfaces;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Common;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Category;

namespace RetailCore.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<CategorySummaryDto>> GetPagedAsync(GetCategoriesRequest request)
    {
        var (items, totalCount) = await _repo.GetPagedAsync(
            request.Keyword,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(c => new CategorySummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description
        });

        return new PagedResult<CategorySummaryDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<CategoryDetailDto>> GetPagedForManagementAsync(GetCategoriesRequest request)
    {
        var (items, totalCount) = await _repo.GetPagedAsync(
            request.Keyword,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var dtos = items.Select(MapToCategoryDetailDto);

        return new PagedResult<CategoryDetailDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CategoryDetailDto> GetByIdAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category id '{id}' not found.");

        return MapToCategoryDetailDto(category);
    }

    public async Task<CategoryDetailDto> GetBySlugAsync(string slug)
    {
        var category = await _repo.GetBySlugAsync(slug)
            ?? throw new KeyNotFoundException($"Category slug '{slug}' not found.");

        return MapToCategoryDetailDto(category);
    }

    public async Task<Guid> CreateAsync(CreateCategoryRequest request)
    {
        if (!await _repo.IsSlugUniqueAsync(request.Slug))
            throw new InvalidOperationException($"Category slug '{request.Slug}' already exists.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Status = CategoryStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(category);

        return category.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category id '{id}' not found.");

        if (category.Slug != request.Slug && !await _repo.IsSlugUniqueAsync(request.Slug, id))
            throw new InvalidOperationException($"Category slug '{request.Slug}' already exists.");

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;
        category.Status = request.Status;
        category.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category id '{id}' not found.");

        await _repo.DeleteAsync(category);
    }

    public static CategoryDetailDto MapToCategoryDetailDto(Category c)
    {
        return new CategoryDetailDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
        };
    }
}
