using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/brands")]
public class AdminBrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public AdminBrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedForManagement([FromQuery] GetBrandsRequest request)
    {
        var brands = await _brandService.GetPagedForManagementAsync(request);
        return Ok(brands);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        return Ok(brand);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var brand = await _brandService.GetBySlugAsync(slug);
        return Ok(brand);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
    {
        var id = await _brandService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request)
    {
        await _brandService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _brandService.DeleteAsync(id);
        return NoContent();
    }
}
