using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.ToTable("Movements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PreviousStatus).HasMaxLength(30);
        builder.Property(x => x.NewStatus).HasMaxLength(30);
        builder.Property(x => x.ReferenceType).HasMaxLength(30);
        builder.Property(x => x.Observations).HasMaxLength(500);
        builder.HasIndex(x => x.ResourceId);
        builder.HasIndex(x => x.PerformedAt);
        builder.HasIndex(x => x.Type);
    }
}
