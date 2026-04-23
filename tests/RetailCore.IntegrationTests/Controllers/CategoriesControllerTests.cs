using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RetailCore.IntegrationTests.Infrastructure;
using RetailCore.Shared.DTOs.Category;

namespace RetailCore.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Categories");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_ShouldCreateCategory()
    {
        var dto = new CategoryCreateDto
        {
            Name = "Test Category"
        };

        var response = await _client.PostAsJsonAsync("/api/categories", dto);

        Assert.True(response.IsSuccessStatusCode);
    }
}