using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("PRODUCTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.Slug).HasColumnName("SLUG").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Name).HasColumnName("NAME").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Description).HasColumnName("DESCRIPTION");
        builder.Property(x => x.CategoryId).HasColumnName("CATEGORY_ID");
        builder.Property(x => x.BasePrice).HasColumnName("BASE_PRICE").HasPrecision(18, 2);
        builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).HasColumnName("IS_DELETED").HasDefaultValue(false);
        builder.Property(x => x.SeoTitle).HasColumnName("SEO_TITLE").HasMaxLength(200);
        builder.Property(x => x.SeoDescription).HasColumnName("SEO_DESCRIPTION").HasMaxLength(500);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("UQ_PRODUCTS_SLUG");
        builder.HasIndex(x => new { x.CategoryId, x.IsActive, x.IsDeleted }).HasDatabaseName("IX_PRODUCTS_CATEGORY");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_PRODUCTS_NAME");

        builder.HasOne(x => x.Category)
               .WithMany()
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
