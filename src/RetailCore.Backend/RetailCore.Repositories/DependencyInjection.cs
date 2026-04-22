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

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
