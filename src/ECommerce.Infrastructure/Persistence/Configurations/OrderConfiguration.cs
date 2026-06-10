using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("ORDERS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.OrderNumber).HasColumnName("ORDER_NUMBER").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.Status).HasColumnName("STATUS").HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pending");
        builder.Property(x => x.Subtotal).HasColumnName("SUBTOTAL").HasPrecision(18, 2);
        builder.Property(x => x.ShippingFee).HasColumnName("SHIPPING_FEE").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(x => x.Discount).HasColumnName("DISCOUNT").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(x => x.Total).HasColumnName("TOTAL").HasPrecision(18, 2);
        builder.Property(x => x.CouponCode).HasColumnName("COUPON_CODE").HasMaxLength(50);
        builder.Property(x => x.ShippingAddressJson).HasColumnName("SHIPPING_ADDRESS_JSON").IsRequired();
        builder.Property(x => x.Note).HasColumnName("NOTE").HasMaxLength(500);
        builder.Property(x => x.PaymentMethod).HasColumnName("PAYMENT_METHOD").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.ConfirmedAt).HasColumnName("CONFIRMED_AT");
        builder.Property(x => x.ShippedAt).HasColumnName("SHIPPED_AT");
        builder.Property(x => x.DeliveredAt).HasColumnName("DELIVERED_AT");
        builder.Property(x => x.CompletedAt).HasColumnName("COMPLETED_AT");
        builder.Property(x => x.CancelledAt).HasColumnName("CANCELLED_AT");
        builder.Property(x => x.CancelReason).HasColumnName("CANCEL_REASON").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("ROW_VERSION").IsRowVersion();
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderNumber).IsUnique().HasDatabaseName("UQ_ORDERS_ORDER_NUMBER");
        builder.HasIndex(x => new { x.UserId, x.CreatedAt }).HasDatabaseName("IX_ORDERS_USER");
        builder.HasIndex(x => new { x.Status, x.CreatedAt }).HasDatabaseName("IX_ORDERS_STATUS");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}
