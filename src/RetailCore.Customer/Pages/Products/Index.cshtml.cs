using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;

namespace RetailCore.Customer.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ECommerceService _service;
    public List<ProductModel> Products { get; set; } = new();
    public List<CategoryModel> Categories { get; set; } = new();
    public string? SelectedCategory { get; set; }
    public string SortBy { get; set; } = "popularity";

    public IndexModel(ECommerceService service)
    {
        _service = service;
    }

    public void OnGet(string? category, string sortBy = "popularity")
    {
        Products = _service.GetProducts();
        Categories = _service.GetCategories();
        SelectedCategory = category;
        SortBy = sortBy;

        // Filter by category
        if (!string.IsNullOrEmpty(category))
        {
            Products = Products.Where(p => p.Category.Slug == category || p.Category.Name == category).ToList();
        }

        // Sort
        Products = sortBy switch
        {
            "price_low" => Products.OrderBy(p => p.Variants.FirstOrDefault()?.Price ?? 0).ToList(),
            "price_high" => Products.OrderByDescending(p => p.Variants.FirstOrDefault()?.Price ?? 0).ToList(),
            "rating" => Products.OrderByDescending(p => p.AverageRating).ToList(),
            _ => Products.OrderByDescending(p => p.ReviewCount).ToList()
        };
    }
}
