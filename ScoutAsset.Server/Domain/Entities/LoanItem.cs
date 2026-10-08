using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class LoanItem : BaseEntity
{
    public int LoanId { get; set; }
    public int ResourceId { get; set; }
    public string ConditionAtDelivery { get; set; } = string.Empty;
    public string? ConditionAtReturn { get; set; }
    public string? ReturnObservations { get; set; }
    public string? DamagesDetected { get; set; }

    public Loan Loan { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
}
