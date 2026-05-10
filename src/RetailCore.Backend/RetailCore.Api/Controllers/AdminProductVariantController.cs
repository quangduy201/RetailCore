using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/products/{productId:guid}/variants")]
public class AdminProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _variantService;

    public AdminProductVariantsController(IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        var result = await _variantService.GetByProductIdAsync(productId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _variantService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid productId, [FromBody] CreateProductVariantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku))
            throw new InvalidOperationException("SKU is required.");

        if (request.Price < 0)
            throw new InvalidOperationException("Price cannot be negative.");

        if (request.Stock < 0)
            throw new InvalidOperationException("Stock cannot be negative.");

        if (request.AttributeValueIds.Count == 0)
            throw new InvalidOperationException("Variant must contain attribute values.");

        var id = await _variantService.CreateAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductVariantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku))
            throw new InvalidOperationException("SKU is required.");

        if (request.Price < 0)
            throw new InvalidOperationException("Price cannot be negative.");

        if (request.Stock < 0)
            throw new InvalidOperationException("Stock cannot be negative.");

        if (request.AttributeValueIds.Count == 0)
            throw new InvalidOperationException("Variant must contain attribute values.");

        await _variantService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _variantService.DeleteDraftAsync(id);
        return NoContent();
    }
}
