using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("PRODUCT_VARIANTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.ProductId).HasColumnName("PRODUCT_ID");
        builder.Property(x => x.Sku).HasColumnName("SKU").HasMaxLength(100).IsRequired();
        builder.Property(x => x.AttributesJson).HasColumnName("ATTRIBUTES_JSON");
        builder.Property(x => x.Price).HasColumnName("PRICE").HasPrecision(18, 2);
        builder.Property(x => x.ComparePrice).HasColumnName("COMPARE_PRICE").HasPrecision(18, 2);
        builder.Property(x => x.ImageUrl).HasColumnName("IMAGE_URL").HasMaxLength(500);
        builder.Property(x => x.Weight).HasColumnName("WEIGHT").HasPrecision(10, 2);
        builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.Sku).IsUnique().HasDatabaseName("UQ_PRODUCT_VARIANTS_SKU");
        builder.HasIndex(x => x.ProductId).HasDatabaseName("IX_PRODUCT_VARIANTS_PRODUCT");

        builder.HasOne(x => x.Product)
               .WithMany(p => p.Variants)
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
