using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Options;
using RetailCore.Repositories.Repositories;
using RetailCore.Repositories.Repositories.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, DatabaseOptions options)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(options.ConnectionString));

        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
        services.AddScoped<IProductVariantImageRepository, ProductVariantImageRepository>();

        return services;
    }
}
