using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailCore.Customer.Services.Api;
using RetailCore.Shared.DTOs.Category;

namespace RetailCore.Customer.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryApi _categoryApi;

    public List<CategorySummaryDto> Categories { get; set; } = [];
    public string? Keyword { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public IndexModel(ICategoryApi categoryApi)
    {
        _categoryApi = categoryApi;
    }

    public async Task OnGetAsync(int p = 1, string? keyword = null)
    {
        CurrentPage = p <= 0 ? 1 : p;
        Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();

        var result = await _categoryApi.GetCategoriesAsync(CurrentPage, PageSize, Keyword);

        Categories = result.Items.ToList();
        TotalPages = result.TotalPages;
        TotalCount = result.TotalCount;
    }
}
