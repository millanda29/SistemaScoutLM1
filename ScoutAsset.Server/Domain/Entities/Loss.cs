using ScoutAsset.Server.Domain.Common;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Domain.Entities;

public class Loss : BaseEntity
{
    public int ResourceId { get; set; }
    public string LossNumber { get; set; } = string.Empty;
    public string Status { get; set; } = nameof(LossStatus.EN_INVESTIGACION);
    public string Circumstances { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportedById { get; set; } = string.Empty;
    public string? ConfirmedById { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public string? Observations { get; set; }

    public Resource Resource { get; set; } = null!;
}
