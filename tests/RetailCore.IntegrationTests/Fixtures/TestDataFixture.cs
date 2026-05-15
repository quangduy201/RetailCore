using RetailCore.Repositories.Data;
using RetailCore.Repositories.Entities;
using RetailCore.Shared.Constants;
using RetailCore.Shared.Enums;

namespace RetailCore.IntegrationTests.Fixtures;

public static class TestDataSeeder
{
    public static async Task<AppRole> SeedRoleAsync(
        AppDbContext context,
        string roleName)
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        };

        context.Roles.Add(role);

        await context.SaveChangesAsync();

        return role;
    }

    public static async Task<AppUser> SeedUserAsync(
        AppDbContext context,
        string email,
        string fullName,
        bool isActive = true,
        DateTime? createdAt = null)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            FullName = fullName,
            AvatarUrl = "avatar.png",
            IsActive = isActive,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        return user;
    }

    public static async Task SeedUserRoleAsync(
        AppDbContext context,
        AppUser user,
        AppRole role)
    {
        context.UserRoles.Add(new AppUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync();
    }

    public static async Task<(AppUser User, AppRole Role)> SeedCustomerAsync(
        AppDbContext context,
        string email = "customer@example.com",
        string fullName = "Customer User")
    {
        var role = await SeedRoleAsync(context, RoleConstants.Customer);

        var user = await SeedUserAsync(
            context,
            email,
            fullName);

        await SeedUserRoleAsync(context, user, role);

        return (user, role);
    }

    public static async Task<(AppUser User, AppRole Role)> SeedAdminAsync(
        AppDbContext context,
        string email = "admin@example.com",
        string fullName = "Admin User")
    {
        var role = await SeedRoleAsync(context, RoleConstants.Admin);

        var user = await SeedUserAsync(
            context,
            email,
            fullName);

        await SeedUserRoleAsync(context, user, role);

        return (user, role);
    }

    public static async Task<Brand> SeedBrandAsync(
        AppDbContext context,
        string name = "Apple",
        BrandStatus status = BrandStatus.Active)
    {
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLowerInvariant().Replace(" ", "-"),
            Description = $"{name} description",
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.Brands.Add(brand);

        await context.SaveChangesAsync();

        return brand;
    }

    public static async Task<Category> SeedCategoryAsync(
        AppDbContext context,
        string name = "Phones",
        CategoryStatus status = CategoryStatus.Active)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLowerInvariant().Replace(" ", "-"),
            Description = $"{name} description",
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.Categories.Add(category);

        await context.SaveChangesAsync();

        return category;
    }

    public static async Task<Product> SeedProductAsync(
        AppDbContext context,
        Brand? brand = null,
        Category? category = null,
        string name = "iPhone 15",
        ProductStatus status = ProductStatus.Active)
    {
        brand ??= await SeedBrandAsync(context);
        category ??= await SeedCategoryAsync(context);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLowerInvariant().Replace(" ", "-"),
            ShortDescription = "Short description",
            Description = "Full description",
            BrandId = brand.Id,
            CategoryId = category.Id,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        return product;
    }

    public static async Task<ProductVariant> SeedVariantAsync(
        AppDbContext context,
        Product? product = null,
        string sku = "SKU-1",
        ProductVariantStatus status = ProductVariantStatus.Active)
    {
        product ??= await SeedProductAsync(context);

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = sku,
            Name = "Variant Name",
            Description = "Variant Description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.ProductVariants.Add(variant);

        await context.SaveChangesAsync();

        return variant;
    }

    public static async Task<RefreshToken> SeedRefreshTokenAsync(
        AppDbContext context,
        AppUser? user = null,
        bool isRevoked = false,
        bool isExpired = false)
    {
        if (user == null)
        {
            var result = await SeedCustomerAsync(context);
            user = result.User;
        }

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = isExpired
                ? DateTime.UtcNow.AddDays(-1)
                : DateTime.UtcNow.AddDays(7),
            IsRevoked = isRevoked,
            RevokedAtUtc = isRevoked
                ? DateTime.UtcNow
                : null
        };

        context.RefreshTokens.Add(token);

        await context.SaveChangesAsync();

        return token;
    }
}
