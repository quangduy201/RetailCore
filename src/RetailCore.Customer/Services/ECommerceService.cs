namespace RetailCore.Customer.Services;

public class ECommerceService
{
    public List<ProductModel> GetProducts() => new()
    {
        SampleData.productModels[0],
        SampleData.productModels[1],
        SampleData.productModels[2],
        SampleData.productModels[3],
        SampleData.productModels[4],
        SampleData.productModels[5],
        SampleData.productModels[6],
        SampleData.productModels[7]
    };

    public List<CategoryModel> GetCategories() => new()
    {
        SampleData.categoryModels[0],
        SampleData.categoryModels[1],
        SampleData.categoryModels[2],
        SampleData.categoryModels[3],
        SampleData.categoryModels[4],
        SampleData.categoryModels[5],
        SampleData.categoryModels[6],
    };
}

// Models
public class BrandModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Slug { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive
}

public class CategoryModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Status { get; set; } = "Active"; // Active, Inactive
    public string Icon => "📦"; // Default icon
}

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public BrandModel Brand { get; set; } = new();
    public CategoryModel Category { get; set; } = new();
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string Status { get; set; } = "Active";
    public List<string> Images { get; set; } = new();
    public List<ProductVariantModel> Variants { get; set; } = new();
}

public class ProductVariantModel
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Status { get; set; } = "Active"; // Active, OutOfStock, Discontinued
    public Dictionary<string, string> Attributes { get; set; } = new(); // Color, Size, etc.
}
