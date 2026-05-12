using Microsoft.Extensions.DependencyInjection;
using RetailCore.Services.Implementations;
using RetailCore.Services.Interfaces;
using RetailCore.Services.Options;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, JwtOptions jwtOptions, AdminOptions adminOptions)
    {
        services.AddSingleton(jwtOptions);
        services.AddSingleton(adminOptions);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentitySeederService, IdentitySeederService>();

        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();

        return services;
    }
}
