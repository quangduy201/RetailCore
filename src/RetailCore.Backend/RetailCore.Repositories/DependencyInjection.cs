using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Options;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, DatabaseOptions options)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(options.ConnectionString));

        return services;
    }
}
