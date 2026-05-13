using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Customer.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductApi _productApi;
    private readonly ICategoryApi _categoryApi;
    private readonly IBrandApi _brandApi;

    public List<ProductSummaryDto> Products { get; set; } = [];
    public List<CategorySummaryDto> Categories { get; set; } = [];
    public List<BrandSummaryDto> Brands { get; set; } = [];

    public string? SelectedCategorySlug { get; set; }
    public string? SelectedBrandSlug { get; set; }
    public string? Keyword { get; set; }

    // Pagination
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 9;

    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public IndexModel(
        IProductApi productApi,
        ICategoryApi categoryApi,
        IBrandApi brandApi)
    {
        _productApi = productApi;
        _categoryApi = categoryApi;
        _brandApi = brandApi;
    }

    public async Task OnGetAsync(
        int p = 1,
        string? category = null,
        string? brand = null,
        string? keyword = null)
    {
        CurrentPage = p <= 0 ? 1 : p;

        SelectedCategorySlug = string.IsNullOrWhiteSpace(category)
            ? null
            : category.Trim();
        SelectedBrandSlug = string.IsNullOrWhiteSpace(brand)
            ? null
            : brand.Trim();
        Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();

        var productResult =
            await _productApi.GetProductsAsync(
                CurrentPage,
                PageSize,
                Keyword,
                SelectedBrandSlug,
                SelectedCategorySlug);

        Products = productResult.Items.ToList();

        TotalPages = productResult.TotalPages;
        TotalCount = productResult.TotalCount;

        var categoryResult =
            await _categoryApi.GetCategoriesAsync(1, 100);

        Categories = categoryResult.Items.ToList();

        var brandResult =
            await _brandApi.GetBrandsAsync(1, 100);

        Brands = brandResult.Items.ToList();
    }
}
