using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("ORDER_ITEMS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderId).HasColumnName("ORDER_ID");
        builder.Property(x => x.VariantId).HasColumnName("VARIANT_ID");
        builder.Property(x => x.ProductNameSnapshot).HasColumnName("PRODUCT_NAME_SNAPSHOT").HasMaxLength(300).IsRequired();
        builder.Property(x => x.VariantSkuSnapshot).HasColumnName("VARIANT_SKU_SNAPSHOT").HasMaxLength(100).IsRequired();
        builder.Property(x => x.VariantAttributesSnapshot).HasColumnName("VARIANT_ATTRIBUTES_SNAPSHOT");
        builder.Property(x => x.UnitPriceSnapshot).HasColumnName("UNIT_PRICE_SNAPSHOT").HasPrecision(18, 2);
        builder.Property(x => x.Quantity).HasColumnName("QUANTITY");
        builder.Property(x => x.LineTotal).HasColumnName("LINE_TOTAL").HasPrecision(18, 2);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_ORDER_ITEMS_ORDER");

        builder.HasOne(x => x.Order).WithMany(o => o.Items).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.NoAction);
    }
}
