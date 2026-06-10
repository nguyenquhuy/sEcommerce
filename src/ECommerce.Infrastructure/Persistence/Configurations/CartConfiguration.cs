using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("CARTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.SessionId).HasColumnName("SESSION_ID").HasMaxLength(200);
        builder.Property(x => x.CouponId).HasColumnName("COUPON_ID");
        builder.Property(x => x.ExpiresAt).HasColumnName("EXPIRES_AT");
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_CARTS_USER");
        builder.HasIndex(x => x.SessionId).HasDatabaseName("IX_CARTS_SESSION");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Coupon).WithMany().HasForeignKey(x => x.CouponId).OnDelete(DeleteBehavior.NoAction);
    }
}
