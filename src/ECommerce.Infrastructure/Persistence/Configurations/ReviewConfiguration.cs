using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("REVIEWS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.ProductId).HasColumnName("PRODUCT_ID");
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.OrderItemId).HasColumnName("ORDER_ITEM_ID");
        builder.Property(x => x.Rating).HasColumnName("RATING");
        builder.Property(x => x.Comment).HasColumnName("COMMENT").HasMaxLength(2000);
        builder.Property(x => x.IsApproved).HasColumnName("IS_APPROVED").HasDefaultValue(false);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.OrderItemId).IsUnique().HasDatabaseName("UQ_REVIEWS_ORDER_ITEM");
        builder.HasIndex(x => new { x.ProductId, x.IsApproved }).HasDatabaseName("IX_REVIEWS_PRODUCT");

        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.NoAction);
    }
}
