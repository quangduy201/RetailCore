using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Repositories.Entities;

namespace RetailCore.Repositories.Configurations;

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(a => a.Values)
            .HasForeignKey(x => x.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate values per attribute
        builder.HasIndex(x => new { x.ProductAttributeId, x.Value })
            .IsUnique();
    }
}
