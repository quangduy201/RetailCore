using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetProductsRequest request)
    {
        request.Status = ProductStatus.Active;
        var products = await _productService.GetPagedAsync(request);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product.Status != ProductStatus.Active)
            throw new KeyNotFoundException($"Product id '{id}' not found.");
        return Ok(product);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var product = await _productService.GetBySlugAsync(slug);
        if (product.Status != ProductStatus.Active)
            throw new KeyNotFoundException($"Product slug '{slug}' not found.");
        return Ok(product);
    }
}
