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
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Product name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new InvalidOperationException("Product slug is required.");

        if (request.BrandId == Guid.Empty)
            throw new InvalidOperationException("BrandId is required.");

        if (request.CategoryId == Guid.Empty)
            throw new InvalidOperationException("CategoryId is required.");

        var id = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Product name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new InvalidOperationException("Product slug is required.");

        if (request.BrandId == Guid.Empty)
            throw new InvalidOperationException("BrandId is required.");

        if (request.CategoryId == Guid.Empty)
            throw new InvalidOperationException("CategoryId is required.");

        await _productService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        return NoContent();
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        return NoContent();
    }

    [HttpPost("{id:guid}/feature")]
    public async Task<IActionResult> Feature(Guid id)
    {
        return NoContent();
    }

    [HttpPost("{id:guid}/unfeature")]
    public async Task<IActionResult> Unfeature(Guid id)
    {
        return NoContent();
    }

    [HttpPatch("{id:guid}/setup-progress")]
    public async Task<IActionResult> SetupProgress(Guid id)
    {
        return NoContent();
    }

    [HttpGet("{id:guid}/setup-status")]
    public async Task<IActionResult> SetupStatus(Guid id)
    {
        return NoContent();
    }
}
