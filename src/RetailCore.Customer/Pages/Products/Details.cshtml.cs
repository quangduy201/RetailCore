using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services;

namespace RetailCore.Customer.Pages.Products;

public class DetailsModel : PageModel
{
    private readonly ECommerceService _service;
    public ProductModel? Product { get; set; }

    public DetailsModel(ECommerceService service)
    {
        _service = service;
    }

    public IActionResult OnGet(string slug)
    {
        Product = _service.GetProducts().FirstOrDefault(p => p.Slug == slug || p.Name == slug);
        if (Product == null)
            return NotFound();
        return Page();
    }
}
