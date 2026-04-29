using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;

namespace RetailCore.Customer.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ProductService _productService;

    public List<ProductModel> Products { get; set; } = new();
    public List<CategoryModel> Categories { get; set; } = new();
    public string? SelectedCategory { get; set; }
    public string? SortBy { get; set; }

    // Pagination properties
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public IndexModel(ProductService productService)
    {
        _productService = productService;
    }

    public async Task OnGetAsync(int p = 1, string? category = null, string? sortBy = null)
    {
        CurrentPage = p <= 0 ? 1 : p;
        SelectedCategory = string.IsNullOrEmpty(category) ? null : category;
        SortBy = string.IsNullOrEmpty(sortBy) ? "popularity" : sortBy;

        // Get paginated products
        var paginationResult = await _productService.GetProductsPagedAsync(
            CurrentPage,
            PageSize,
            SelectedCategory,
            SortBy
        );

        Products = paginationResult.Items;
        TotalPages = paginationResult.TotalPages;
        TotalCount = paginationResult.TotalCount;
        HasPreviousPage = paginationResult.HasPreviousPage;
        HasNextPage = paginationResult.HasNextPage;

        // Get categories
        Categories = await _productService.GetCategoriesAsync();
    }
}
