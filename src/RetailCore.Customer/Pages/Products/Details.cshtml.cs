using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Product;

namespace RetailCore.Customer.Pages.Products;

public class DetailsModel : PageModel
{
    private readonly IProductApi _productApi;
    public ProductDetailDto? Product { get; set; }

    public DetailsModel(IProductApi productApi)
    {
        _productApi = productApi;
    }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        try
        {
            Product = await _productApi.GetBySlugAsync(slug);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}