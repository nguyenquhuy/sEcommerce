using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("INVENTORIES");

        builder.HasKey(x => x.VariantId);
        builder.Property(x => x.VariantId).HasColumnName("VARIANT_ID").ValueGeneratedNever();
        builder.Property(x => x.OnHand).HasColumnName("ON_HAND").HasDefaultValue(0);
        builder.Property(x => x.Reserved).HasColumnName("RESERVED").HasDefaultValue(0);
        builder.Property(x => x.Available).HasColumnName("AVAILABLE")
               .HasComputedColumnSql("[ON_HAND] - [RESERVED]", stored: true);
        builder.Property(x => x.RowVersion).HasColumnName("ROW_VERSION").IsRowVersion();
        builder.ConfigureAuditColumns();

        builder.HasOne(x => x.Variant)
               .WithOne(v => v.Inventory)
               .HasForeignKey<Inventory>(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
