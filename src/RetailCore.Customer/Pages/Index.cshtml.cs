using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;

namespace RetailCore.Customer.Pages;

public class IndexModel : PageModel
{
    private readonly ECommerceService _service;
    public List<ProductModel> FeaturedProducts { get; set; } = new();
    public List<CategoryModel> Categories { get; set; } = new();

    public IndexModel(ECommerceService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        var allProducts = _service.GetProducts();
        FeaturedProducts = allProducts.Take(4).ToList();
        Categories = _service.GetCategories();
    }
}
