using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class PhysicalInventoryConfiguration : IEntityTypeConfiguration<PhysicalInventory>
{
    public void Configure(EntityTypeBuilder<PhysicalInventory> builder)
    {
        builder.ToTable("PhysicalInventories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InventoryNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Observations).HasMaxLength(2000);
        builder.HasIndex(x => x.InventoryNumber).IsUnique();
    }
}
