using Microsoft.Extensions.DependencyInjection;
using RetailCore.Services.Implementations;
using RetailCore.Services.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
