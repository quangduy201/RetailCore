namespace RetailCore.Shared.DTOs.Product;

public class ProductCreateDto
{
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
