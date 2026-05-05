using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Brand;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetBrandsRequest request)
    {
        request.Status = BrandStatus.Active;
        var brands = await _brandService.GetPagedAsync(request);
        return Ok(brands);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand.Status != BrandStatus.Active)
            throw new KeyNotFoundException($"Brand id '{id}' not found.");
        return Ok(brand);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var brand = await _brandService.GetBySlugAsync(slug);
        if (brand.Status != BrandStatus.Active)
            throw new KeyNotFoundException($"Brand slug '{slug}' not found.");
        return Ok(brand);
    }
}
