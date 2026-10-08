using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequestNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ApprovalType).HasMaxLength(20);
        builder.Property(x => x.ApprovalObservations).HasMaxLength(500);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.HasIndex(x => x.RequestNumber).IsUnique();
        builder.HasIndex(x => x.Status);
    }
}
