using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Brand;
using RetailCore.Shared.DTOs.Category;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Customer.Pages;

public class IndexModel : PageModel
{
    private readonly IProductApi _productApi;
    private readonly ICategoryApi _categoryApi;
    private readonly IBrandApi _brandApi;

    public List<ProductSummaryDto> LatestProducts { get; set; } = [];
    public List<CategorySummaryDto> Categories { get; set; } = [];
    public List<BrandSummaryDto> Brands { get; set; } = [];

    public IndexModel(
        IProductApi productApi,
        ICategoryApi categoryApi,
        IBrandApi brandApi)
    {
        _productApi = productApi;
        _categoryApi = categoryApi;
        _brandApi = brandApi;
    }

    public async Task OnGetAsync()
    {
        LatestProducts =
            (await _productApi.GetProductsAsync(1, 4))
            .Items
            .ToList();

        Categories =
            (await _categoryApi.GetCategoriesAsync(1, 8))
            .Items
            .ToList();

        Brands =
            (await _brandApi.GetBrandsAsync(1, 8))
            .Items
            .ToList();
    }
}
