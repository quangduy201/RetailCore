using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Services.Options;
using RetailCore.Shared.Constants;

namespace RetailCore.IntegrationTests.Fixtures;

public class RetailCoreApiFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private SqliteConnection? _connection;

    public RetailCoreApiFactory()
    {
        Environment.SetEnvironmentVariable("Database__ConnectionString", "TestDb");

        Environment.SetEnvironmentVariable("Jwt__Issuer", "RetailCore.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "RetailCore.Tests.Client");
        Environment.SetEnvironmentVariable("Jwt__SecretKey", "this-is-a-very-long-secret-key-for-integration-tests-only");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationInSeconds", "3600");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationInSeconds", "86400");

        Environment.SetEnvironmentVariable("Admin__Email", "admin@example.com");
        Environment.SetEnvironmentVariable("Admin__Password", "Admin123!");
        Environment.SetEnvironmentVariable("Admin__FullName", "System Admin");
        Environment.SetEnvironmentVariable("Admin__AvatarUrl", "admin-avatar.png");

        Environment.SetEnvironmentVariable("Cors__AllowedOrigins__0", "http://localhost:5173");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            services.RemoveAll<JwtOptions>();
            services.RemoveAll<AdminOptions>();

            services.AddSingleton(new JwtOptions
            {
                Issuer = "RetailCore.Tests",
                Audience = "RetailCore.Tests.Client",
                SecretKey = "this-is-a-very-long-secret-key-for-integration-tests-only",
                AccessTokenExpirationInSeconds = 3600,
                RefreshTokenExpirationInSeconds = 86400
            });

            services.AddSingleton(new AdminOptions
            {
                Email = "admin@example.com",
                Password = "Admin123!",
                FullName = "System Admin",
                AvatarUrl = "admin-avatar.png"
            });

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Roles.AddRange(
                new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = RoleConstants.Admin,
                    NormalizedName = RoleConstants.Admin.ToUpperInvariant()
                },
                new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = RoleConstants.Customer,
                    NormalizedName = RoleConstants.Customer.ToUpperInvariant()
                });

            db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Dispose();

        Environment.SetEnvironmentVariable("Database__ConnectionString", null);

        Environment.SetEnvironmentVariable("Jwt__Issuer", null);
        Environment.SetEnvironmentVariable("Jwt__Audience", null);
        Environment.SetEnvironmentVariable("Jwt__SecretKey", null);
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationInSeconds", null);
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationInSeconds", null);

        Environment.SetEnvironmentVariable("Admin__Email", null);
        Environment.SetEnvironmentVariable("Admin__Password", null);
        Environment.SetEnvironmentVariable("Admin__FullName", null);
        Environment.SetEnvironmentVariable("Admin__AvatarUrl", null);

        Environment.SetEnvironmentVariable("Cors__AllowedOrigins__0", null);
    }
}
