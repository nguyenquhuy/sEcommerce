using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.ToTable("RETURN_REQUESTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderId).HasColumnName("ORDER_ID");
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.Status).HasColumnName("STATUS").HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pending");
        builder.Property(x => x.Reason).HasColumnName("REASON").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.StaffNote).HasColumnName("STAFF_NOTE").HasMaxLength(1000);
        builder.Property(x => x.RefundAmount).HasColumnName("REFUND_AMOUNT").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_RETURN_REQUESTS_ORDER");

        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.NoAction);
    }
}
