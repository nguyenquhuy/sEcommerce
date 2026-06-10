using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USERS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID").HasDefaultValueSql("NEWID()");
        builder.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(256).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("PASSWORD_HASH").HasMaxLength(500).IsRequired();
        builder.Property(x => x.FullName).HasColumnName("FULL_NAME").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("PHONE").HasMaxLength(20);
        builder.Property(x => x.Role).HasColumnName("ROLE").HasMaxLength(20).IsUnicode(false).HasDefaultValue("Customer");
        builder.Property(x => x.IsEmailVerified).HasColumnName("IS_EMAIL_VERIFIED").HasDefaultValue(false);
        builder.Property(x => x.EmailVerifyToken).HasColumnName("EMAIL_VERIFY_TOKEN").HasMaxLength(200);
        builder.Property(x => x.EmailVerifyExpiry).HasColumnName("EMAIL_VERIFY_EXPIRY");
        builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE").HasDefaultValue(true);
        builder.Property(x => x.LoyaltyPoint).HasColumnName("LOYALTY_POINT").HasDefaultValue(0);
        builder.Property(x => x.LastLoginAt).HasColumnName("LAST_LOGIN_AT");
        builder.ConfigureAuditColumns();

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQ_USERS_EMAIL");
    }
}
