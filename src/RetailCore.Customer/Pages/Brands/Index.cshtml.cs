using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Brand;

namespace RetailCore.Customer.Pages.Brands;

public class IndexModel : PageModel
{
    private readonly IBrandApi _brandApi;

    public List<BrandSummaryDto> Brands { get; set; } = [];
    public string? Keyword { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public IndexModel(IBrandApi brandApi)
    {
        _brandApi = brandApi;
    }

    public async Task OnGetAsync(int p = 1, string? keyword = null)
    {
        CurrentPage = p <= 0 ? 1 : p;
        Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();

        var result = await _brandApi.GetBrandsAsync(CurrentPage, PageSize, Keyword);

        Brands = result.Items.ToList();
        TotalPages = result.TotalPages;
        TotalCount = result.TotalCount;
    }
}
