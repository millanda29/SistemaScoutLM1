using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class LoanItemConfiguration : IEntityTypeConfiguration<LoanItem>
{
    public void Configure(EntityTypeBuilder<LoanItem> builder)
    {
        builder.ToTable("LoanItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConditionAtDelivery).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ConditionAtReturn).HasMaxLength(20);
        builder.Property(x => x.ReturnObservations).HasMaxLength(500);
        builder.Property(x => x.DamagesDetected).HasMaxLength(1000);
        builder.HasIndex(x => new { x.LoanId, x.ResourceId }).IsUnique();
    }
}
