using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Result).HasMaxLength(500);
        builder.Property(x => x.Observations).HasMaxLength(1000);
        builder.Property(x => x.Cost).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.ResourceId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ScheduledDate);
    }
}
