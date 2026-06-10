using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderAuditLogConfiguration : IEntityTypeConfiguration<OrderAuditLog>
{
    public void Configure(EntityTypeBuilder<OrderAuditLog> builder)
    {
        builder.ToTable("ORDER_AUDIT_LOGS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderId).HasColumnName("ORDER_ID");
        builder.Property(x => x.FromStatus).HasColumnName("FROM_STATUS").HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.ToStatus).HasColumnName("TO_STATUS").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.ChangedBy).HasColumnName("CHANGED_BY");
        builder.Property(x => x.Reason).HasColumnName("REASON").HasMaxLength(500);
        builder.Property(x => x.ChangedAt).HasColumnName("CHANGED_AT").HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_ORDER_AUDIT_LOGS_ORDER");

        builder.HasOne(x => x.Order).WithMany(o => o.AuditLogs).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ChangedBy).OnDelete(DeleteBehavior.NoAction);
    }
}
