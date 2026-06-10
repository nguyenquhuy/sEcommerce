using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("PAYMENTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderId).HasColumnName("ORDER_ID");
        builder.Property(x => x.Method).HasColumnName("METHOD").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("AMOUNT").HasPrecision(18, 2);
        builder.Property(x => x.Status).HasColumnName("STATUS").HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pending");
        builder.Property(x => x.TxnRef).HasColumnName("TXN_REF").HasMaxLength(100);
        builder.Property(x => x.GatewayResponseJson).HasColumnName("GATEWAY_RESPONSE_JSON");
        builder.Property(x => x.PaidAt).HasColumnName("PAID_AT");
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_PAYMENTS_ORDER");
        builder.HasIndex(x => x.TxnRef).IsUnique().HasFilter("[TXN_REF] IS NOT NULL").HasDatabaseName("UX_PAYMENTS_TXN_REF");

        builder.HasOne(x => x.Order).WithMany(o => o.Payments).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
