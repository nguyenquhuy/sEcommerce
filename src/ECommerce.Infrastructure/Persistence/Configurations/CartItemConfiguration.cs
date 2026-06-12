using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CART_ITEMS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.CartId).HasColumnName("CART_ID");
        builder.Property(x => x.VariantId).HasColumnName("VARIANT_ID");
        builder.Property(x => x.Quantity).HasColumnName("QUANTITY");
        builder.Property(x => x.ReservedQuantity).HasColumnName("RESERVED_QUANTITY").HasDefaultValue(0);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => new { x.CartId, x.VariantId }).IsUnique().HasDatabaseName("UQ_CART_ITEMS_CART_VARIANT");
        builder.HasIndex(x => x.CartId).HasDatabaseName("IX_CART_ITEMS_CART");

        builder.HasOne(x => x.Cart).WithMany(c => c.Items).HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.NoAction);
    }
}
