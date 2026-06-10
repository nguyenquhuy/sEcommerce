using ECommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public static class AuditableEntityConfigurationExtensions
{
    /// <summary>Maps the shared audit columns (CREATED_AT/BY, UPDATED_AT/BY) for any IAuditable entity.</summary>
    public static void ConfigureAuditColumns<T>(this EntityTypeBuilder<T> builder) where T : class, IAuditable
    {
        builder.Property(x => x.CreatedAt).HasColumnName("CREATED_AT").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatedBy).HasColumnName("CREATED_BY");
        builder.Property(x => x.UpdatedAt).HasColumnName("UPDATED_AT");
        builder.Property(x => x.UpdatedBy).HasColumnName("UPDATED_BY");
    }
}
