using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(50);
        builder.Property(x => x.Observations).HasMaxLength(500);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.PerformedAt);
    }
}
