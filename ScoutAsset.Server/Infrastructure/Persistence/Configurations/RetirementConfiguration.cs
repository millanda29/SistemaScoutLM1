using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class RetirementConfiguration : IEntityTypeConfiguration<Retirement>
{
    public void Configure(EntityTypeBuilder<Retirement> builder)
    {
        builder.ToTable("Retirements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RetirementNumber).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Reason).IsRequired();
        builder.Property(x => x.RequestedById).IsRequired();
        builder.Property(x => x.AppraisedValueAtRetirement).HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.ResourceId).IsUnique();
    }
}
