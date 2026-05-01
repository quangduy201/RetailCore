namespace RetailCore.Customer.Services;

public class SampleData
{
    public static List<BrandModel> brandModels =
    [
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000101"),
            Name = "AudioCore",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000102"),
            Name = "KeyStrike",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000103"),
            Name = "CursorPro",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000104"),
            Name = "CableTech",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000105"),
            Name = "ChargePro",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000106"),
            Name = "ErgoPro",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000107"),
            Name = "SurfaceTech",
            Status = "Active"
        },
        new BrandModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000108"),
            Name = "DataStrike",
            Status = "Active"
        },
    ];

    public static List<CategoryModel> categoryModels =
    [
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000201"),
            Name = "Audio",
            Slug = "audio"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000202"),
            Name = "Keyboards",
            Slug = "keyboards"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000203"),
            Name = "Mice",
            Slug = "mice"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000204"),
            Name = "Cables",
            Slug = "cables"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000205"),
            Name = "Chargers",
            Slug = "chargers"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000206"),
            Name = "Accessories",
            Slug = "accessories"
        },
        new CategoryModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000207"),
            Name = "Storage",
            Slug = "storage"
        },
    ];

    public static List<ProductModel> productModels =
    [
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "Wireless Pro Headphones",
            Slug = "wireless-pro-headphones",
            ShortDescription = "Premium wireless headphones with active noise cancellation",
            Description = "Premium wireless headphones with active noise cancellation and 30-hour battery life. Features advanced Bluetooth 5.0 technology and premium sound quality.",
            Brand = brandModels[0],
            Category = categoryModels[0],
            AverageRating = 4.8,
            ReviewCount = 342,
            Status = "Active",
            Images = new()
            {
                "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop",
                "https://images.unsplash.com/photo-1487215078519-e21cc028cb29?w=500&h=500&fit=crop"
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010001"),
                    Sku = "WPH-001-BLK",
                    Price = 199.99m,
                    CompareAtPrice = 249.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Warranty", "2 Years" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010002"),
                    Sku = "WPH-001-SLV",
                    Price = 199.99m,
                    CompareAtPrice = 249.99m,
                    Attributes = new()
                    {
                        { "Color", "Silver" },
                        { "Warranty", "2 Years" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Name = "Mechanical Gaming Keyboard RGB",
            Slug = "mechanical-gaming-keyboard",
            ShortDescription = "RGB mechanical keyboard with hot-swappable switches",
            Description = "Professional-grade RGB mechanical keyboard with hot-swappable switches and programmable keys. Perfect for gaming and professional typing.",
            Brand = brandModels[1],
            Category = categoryModels[1],
            AverageRating = 4.7,
            ReviewCount = 521,
            Status = "Active",
            Images = new()
            {
                "https://images.unsplash.com/photo-1656711081969-9d16ebc2d210?w=500&h=500&fit=crop"
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010003"),
                    Sku = "MGK-001-BLK",
                    Price = 149.99m,
                    CompareAtPrice = 179.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Size", "Full" },
                        { "Switch Type", "Red" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010004"),
                    Sku = "MGK-001-WHT",
                    Price = 149.99m,
                    CompareAtPrice = 179.99m,
                    Attributes = new()
                    {
                        { "Color", "White" },
                        { "Size", "Full" },
                        { "Switch Type", "Blue" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Name = "Precision Gaming Mouse",
            Slug = "precision-gaming-mouse",
            ShortDescription = "High-precision gaming mouse with adjustable DPI",
            Description = "Ergonomic gaming mouse with adjustable DPI up to 16,000 and customizable RGB lighting. Designed for professional gaming.",
            Brand = brandModels[2],
            Category = categoryModels[2],
            AverageRating = 4.6,
            ReviewCount = 284,
            Status = "Active",
            Images = new()
            {
                "https://images.unsplash.com/photo-1727417453138-7d8efdd70fb3?q=500&w=500&fit=crop"
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010005"),
                    Sku = "PGM-001-BLK",
                    Price = 99.99m,
                    CompareAtPrice = 129.99m,
                    Attributes = new()
                    {
                        { "Color", "Matte Black" },
                        { "Size", "Standard" },
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010006"),
                    Sku = "PGM-001-RGB",
                    Price = 99.99m,
                    CompareAtPrice = 129.99m,
                    Attributes = new()
                    {
                        { "Color", "RGB" },
                        { "Size", "Standard" },
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            Name = "USB-C Cable (6ft)",
            Slug = "usb-c-cable-6ft",
            ShortDescription = "Durable USB-C cable with fast charging support",
            Description = "Durable USB-C cable with fast charging support up to 100W. Compatible with all USB-C devices and supports data transfer speeds up to 10Gbps.",
            Brand = brandModels[3],
            Category = categoryModels[3],
            AverageRating = 4.5,
            ReviewCount = 1203,
            Images = new()
            {
                "https://images.unsplash.com/photo-1639675960002-2f414c58ed79?q=500&w=500&fit=crop",
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010007"),
                    Sku = "USBC-001-WHT",
                    Price = 15.99m,
                    CompareAtPrice = 19.99m,
                    Attributes = new()
                    {
                        { "Color", "White" },
                        { "Size", "6ft" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010008"),
                    Sku = "USBC-001-BLK",
                    Price = 15.99m,
                    CompareAtPrice = 19.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Size", "6ft" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010009"),
                    Sku = "USBC-001-RGB",
                    Price = 15.99m,
                    CompareAtPrice = 19.99m,
                    Attributes = new()
                    {
                        { "Color", "RGB" },
                        { "Size", "6ft" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
            Name = "Fast Wireless Charger",
            Slug = "fast-wireless-charger",
            ShortDescription = "15W fast wireless charger compatible with all Qi devices",
            Description = "15W fast wireless charger with elegant design. Compatible with all Qi devices. Features temperature control and foreign object detection for safe charging.",
            Brand = brandModels[4],
            Category = categoryModels[4],
            AverageRating = 4.6,
            ReviewCount = 654,
            Images = new()
            {
                "https://images.unsplash.com/photo-1665525547214-e5995e9af05f?q=500&w=500&fit=crop",
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010010"),
                    Sku = "FAST-CHARGER-WHT",
                    Price = 34.99m,
                    CompareAtPrice = 49.99m,
                    Attributes = new()
                    {
                        { "Color", "White" },
                        { "Size", "Standard" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010011"),
                    Sku = "FAST-CHARGER-BLK",
                    Price = 34.99m,
                    CompareAtPrice = 49.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Size", "Standard" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
            Name = "Laptop Stand",
            Slug = "laptop-stand",
            ShortDescription = "Adjustable aluminum laptop stand for ergonomic setup",
            Description = "Adjustable aluminum laptop stand with multiple height settings for ergonomic setup. Compatible with all laptop sizes and features a sleek, modern design.",
            Brand = brandModels[5],
            Category = categoryModels[5],
            AverageRating = 4.7,
            ReviewCount = 412,
            Images = new()
            {
                "https://images.unsplash.com/photo-1629317480872-45e07211ffd4?q=500&w=500&fit=crop"
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010012"),
                    Sku = "LAPTOP-STAND-SIL",
                    Price = 49.99m,
                    CompareAtPrice = 59.99m,
                    Attributes = new()
                    {
                        { "Color", "Silver" },
                        { "Size", "Adjustable" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010013"),
                    Sku = "LAPTOP-STAND-BLK",
                    Price = 49.99m,
                    CompareAtPrice = 59.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Size", "Adjustable" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
            Name = "Wireless Mouse Pad",
            Slug = "wireless-mouse-pad",
            ShortDescription = "Mouse pad with integrated wireless charging for Qi devices",
            Description = "Mouse pad with integrated wireless charging for Qi devices. Features a non-slip base and smooth surface for precise mouse control.",
            Brand = brandModels[6],
            Category = categoryModels[5],
            AverageRating = 4.4,
            ReviewCount = 189,
            Images = new()
            {
                "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=500&h=500&fit=crop",
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010014"),
                    Sku = "MOUSEPAD-WHT",
                    Price = 29.99m,
                    CompareAtPrice = 39.99m,
                    Attributes = new()
                    {
                        { "Color", "White" },
                        { "Size", "Standard" }
                    },
                    Status = "Active"
                }
            }
        },
        new ProductModel
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000008"),
            Name = "Portable SSD 1TB",
            Slug = "portable-ssd-1tb",
            ShortDescription = "Ultra-fast portable SSD with 1TB storage capacity",
            Description = "Ultra-fast portable SSD with 1TB storage capacity. Features USB 3.2 Gen 2 interface for blazing fast data transfer speeds and a durable, shock-resistant design.",
            Brand = brandModels[7],
            Category = categoryModels[6],
            AverageRating = 4.8,
            ReviewCount = 723,
            Images = new()
            {
                "https://images.unsplash.com/photo-1652105425087-08343fe051f4?q=500&w=500&fit=crop"
            },
            Variants = new()
            {
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010015"),
                    Sku = "SSD-BLK",
                    Price = 99.99m,
                    CompareAtPrice = 129.99m,
                    Attributes = new()
                    {
                        { "Color", "Black" },
                        { "Size", "1TB" }
                    },
                    Status = "Active"
                },
                new ProductVariantModel
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000010016"),
                    Sku = "SSD-SLV",
                    Price = 99.99m,
                    CompareAtPrice = 129.99m,
                    Attributes = new()
                    {
                        { "Color", "Silver" },
                        { "Size", "1TB" }
                    },
                    Status = "Active"
                }
            }
        }
    ];
}