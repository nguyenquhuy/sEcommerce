using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("COUPONS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.Code).HasColumnName("CODE").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Type).HasColumnName("TYPE").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.Value).HasColumnName("VALUE").HasPrecision(18, 2);
        builder.Property(x => x.MinOrderAmount).HasColumnName("MIN_ORDER_AMOUNT").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(x => x.MaxDiscountAmount).HasColumnName("MAX_DISCOUNT_AMOUNT").HasPrecision(18, 2);
        builder.Property(x => x.MaxUsage).HasColumnName("MAX_USAGE");
        builder.Property(x => x.MaxUsagePerUser).HasColumnName("MAX_USAGE_PER_USER").HasDefaultValue(1);
        builder.Property(x => x.UsedCount).HasColumnName("USED_COUNT").HasDefaultValue(0);
        builder.Property(x => x.StartAt).HasColumnName("START_AT");
        builder.Property(x => x.EndAt).HasColumnName("END_AT");
        builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQ_COUPONS_CODE");
    }
}
