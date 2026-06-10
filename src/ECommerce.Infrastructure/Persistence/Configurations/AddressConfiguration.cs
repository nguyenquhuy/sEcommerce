using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("ADDRESSES");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.RecipientName).HasColumnName("RECIPIENT_NAME").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("PHONE").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Province).HasColumnName("PROVINCE").HasMaxLength(100).IsRequired();
        builder.Property(x => x.District).HasColumnName("DISTRICT").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Ward).HasColumnName("WARD").HasMaxLength(100);
        builder.Property(x => x.Street).HasColumnName("STREET").HasMaxLength(300).IsRequired();
        builder.Property(x => x.IsDefault).HasColumnName("IS_DEFAULT").HasDefaultValue(false);
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_ADDRESSES_USER");

        builder.HasOne(x => x.User)
               .WithMany(u => u.Addresses)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
