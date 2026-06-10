using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("CATEGORIES");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.ParentId).HasColumnName("PARENT_ID");
        builder.Property(x => x.Name).HasColumnName("NAME").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("SLUG").HasMaxLength(200).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("SORT_ORDER").HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("UQ_CATEGORIES_SLUG");

        builder.HasOne(x => x.Parent)
               .WithMany(x => x.Children)
               .HasForeignKey(x => x.ParentId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
