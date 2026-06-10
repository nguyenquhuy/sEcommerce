using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("SHIPMENTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderId).HasColumnName("ORDER_ID");
        builder.Property(x => x.Provider).HasColumnName("PROVIDER").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.TrackingNumber).HasColumnName("TRACKING_NUMBER").HasMaxLength(100);
        builder.Property(x => x.Status).HasColumnName("STATUS").HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pending");
        builder.Property(x => x.ShippedAt).HasColumnName("SHIPPED_AT");
        builder.Property(x => x.DeliveredAt).HasColumnName("DELIVERED_AT");
        builder.Property(x => x.CostFee).HasColumnName("COST_FEE").HasPrecision(18, 2);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_SHIPMENTS_ORDER");

        builder.HasOne(x => x.Order).WithMany(o => o.Shipments).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
