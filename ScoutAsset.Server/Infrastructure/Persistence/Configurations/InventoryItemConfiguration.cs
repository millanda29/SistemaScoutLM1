using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Result).HasMaxLength(20).IsRequired();
        builder.Property(x => x.PhysicalCondition).HasMaxLength(20);
        builder.Property(x => x.FoundCode).HasMaxLength(20);
        builder.Property(x => x.FoundName).HasMaxLength(200);
        builder.Property(x => x.Observations).HasMaxLength(1000);
        builder.HasIndex(x => x.PhysicalInventoryId);
        builder.HasIndex(x => x.ResourceId);
    }
}
