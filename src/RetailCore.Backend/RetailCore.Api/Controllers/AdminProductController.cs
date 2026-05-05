using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/products")]
public class AdminProductController : ControllerBase
{
    private readonly IProductService _productService;

    public AdminProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedForManagement([FromQuery] GetProductsRequest request)
    {
        var products = await _productService.GetPagedForManagementAsync(request);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
            throw new InvalidOperationException("Name and Slug are required.");

        if (request.BrandId == Guid.Empty || request.CategoryId == Guid.Empty)
            throw new InvalidOperationException("BrandId and CategoryId are required.");

        var id = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
            throw new InvalidOperationException("Name and Slug are required.");

        if (request.BrandId == Guid.Empty || request.CategoryId == Guid.Empty)
            throw new InvalidOperationException("BrandId and CategoryId are required.");

        await _productService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
