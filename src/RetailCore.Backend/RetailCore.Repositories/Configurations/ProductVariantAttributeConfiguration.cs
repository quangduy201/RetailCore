using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Configurations;

public class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
    {
        builder.ToTable("ProductVariantAttributes");

        // Composite key
        builder.HasKey(x => new { x.ProductVariantId, x.ProductAttributeValueId });

        builder.HasOne(x => x.ProductVariant)
            .WithMany(v => v.Attributes)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductAttributeValue)
            .WithMany(v => v.VariantAttributes)
            .HasForeignKey(x => x.ProductAttributeValueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
