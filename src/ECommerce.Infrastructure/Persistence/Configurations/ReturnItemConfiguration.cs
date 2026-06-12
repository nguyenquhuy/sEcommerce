using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.ToTable("RETURN_ITEMS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.ReturnRequestId).HasColumnName("RETURN_REQUEST_ID");
        builder.Property(x => x.OrderItemId).HasColumnName("ORDER_ITEM_ID");
        builder.Property(x => x.Quantity).HasColumnName("QUANTITY");
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.ReturnRequestId).HasDatabaseName("IX_RETURN_ITEMS_REQUEST");

        builder.HasOne(x => x.ReturnRequest).WithMany(r => r.Items)
               .HasForeignKey(x => x.ReturnRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.OrderItem).WithMany()
               .HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.NoAction);
    }
}
