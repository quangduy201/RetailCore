namespace RetailCore.Shared.DTOs.Product;

public class ProductResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
