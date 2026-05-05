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
        var id = await _variantService.CreateAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductVariantRequest request)
    {
        await _variantService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _variantService.DeleteAsync(id);
        return NoContent();
    }
}
