namespace RetailCore.Customer.Services;

public class ProductService
{
    public async Task<ProductPaginationResult> GetProductsPagedAsync(int page = 1, int pageSize = 9, string? category = null, string? sortBy = null)
    {
        // TODO: Replace mock data with API call to get paginated products from API
        // Mock implementation
        var products = SampleData.productModels.ToList();

        // Filter by category
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category.Slug == category || p.Category.Name == category).ToList();
        }

        // Sort
        products = sortBy switch
        {
            "price_low" => products.OrderBy(p => p.Variants.FirstOrDefault()?.Price ?? 0).ToList(),
            "price_high" => products.OrderByDescending(p => p.Variants.FirstOrDefault()?.Price ?? 0).ToList(),
            "rating" => products.OrderByDescending(p => p.AverageRating).ToList(),
            _ => products.OrderByDescending(p => p.ReviewCount).ToList() // popularity
        };

        // Pagination
        var totalCount = products.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new ProductPaginationResult
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        // TODO: Replace mock data with API call
        return await Task.FromResult(SampleData.categoryModels.ToList());
    }
}

public class ProductPaginationResult
{
    public List<ProductModel> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
