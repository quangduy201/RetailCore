using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/products/{productId:guid}/attributes")]
public class AdminProductAttributeController : ControllerBase
{
    private readonly IProductAttributeService _attributeService;

    public AdminProductAttributeController(IProductAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        var attributes = await _attributeService.GetByProductIdAsync(productId);
        return Ok(attributes);
    }

    [HttpGet("{attributeId:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var attribute = await _attributeService.GetByIdAsync(id);
        return Ok(attribute);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid productId, [FromBody] CreateProductAttributeRequest request)
    {
        var id = await _attributeService.CreateAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, attributeId = id }, id);
    }

    [HttpPut("{attributeId:guid}")]
    public async Task<IActionResult> Update(Guid attributeId, [FromBody] UpdateProductAttributeRequest request)
    {
        await _attributeService.UpdateAsync(attributeId, request);
        return NoContent();
    }

    [HttpDelete("{attributeId:guid}")]
    public async Task<IActionResult> Delete(Guid attributeId)
    {
        await _attributeService.DeleteAsync(attributeId);
        return NoContent();
    }

    [HttpGet("{attributeId:guid}/values")]
    public async Task<IActionResult> GetValues(Guid attributeId)
    {
        var values = await _attributeService.GetValuesByAttributeIdAsync(attributeId);
        return Ok(values);
    }

    [HttpPost("{attributeId:guid}/values")]
    public async Task<IActionResult> CreateValue(Guid attributeId, [FromBody] CreateProductAttributeValueRequest request)
    {
        var id = await _attributeService.CreateValueAsync(attributeId, request);
        return Ok(id);
    }

    [HttpPut("{attributeId:guid}/values/{valueId:guid}")]
    public async Task<IActionResult> UpdateValue(Guid valueId, [FromBody] UpdateProductAttributeValueRequest request)
    {
        await _attributeService.UpdateValueAsync(valueId, request);
        return NoContent();
    }

    [HttpDelete("{attributeId:guid}/values/{valueId:guid}")]
    public async Task<IActionResult> DeleteValue(Guid valueId)
    {
        await _attributeService.DeleteValueAsync(valueId);
        return NoContent();
    }
}
