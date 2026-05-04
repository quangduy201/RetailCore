using Microsoft.Extensions.DependencyInjection;
using RetailCore.Services.Implementations;
using RetailCore.Services.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IProductAttributeService, ProductAttributeService>();

        return services;
    }
}
