using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Constants;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = RoleConstants.Admin)]
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
        await _productService.UpdateStatusAsync(id, ProductStatus.Active);
        return NoContent();
    }

    [HttpPost]
    [Route("{id:guid}/unpublish")]
    [Route("{id:guid}/restore")]
    public async Task<IActionResult> UnpublishOrRestore(Guid id)
    {
        await _productService.UpdateStatusAsync(id, ProductStatus.Inactive);
        return NoContent();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        await _productService.UpdateStatusAsync(id, ProductStatus.Archived);
        return NoContent();
    }
}
