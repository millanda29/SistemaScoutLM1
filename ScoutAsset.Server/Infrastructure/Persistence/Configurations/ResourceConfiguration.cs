using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Model).HasMaxLength(100);
        builder.Property(x => x.SerialNumber).HasMaxLength(100);
        builder.Property(x => x.AcquisitionType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.AcquisitionCost).HasColumnType("decimal(18,2)");
        builder.Property(x => x.AppraisedValue).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PhysicalCondition).HasMaxLength(20).IsRequired();
        builder.Property(x => x.AdministrativeStatus).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Observations).HasMaxLength(2000);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.AdministrativeStatus);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Resources)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Location)
            .WithMany(l => l.Resources)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
