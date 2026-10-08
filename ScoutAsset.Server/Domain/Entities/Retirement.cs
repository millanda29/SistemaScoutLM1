using ScoutAsset.Server.Domain.Common;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Domain.Entities;

public class Retirement : BaseEntity
{
    public int ResourceId { get; set; }
    public string RetirementNumber { get; set; } = string.Empty;
    public string Status { get; set; } = nameof(RetirementStatus.SOLICITADA);
    public string Reason { get; set; } = string.Empty;
    public string RequestedById { get; set; } = string.Empty;
    public string? ReviewedById { get; set; }
    public string? AuthorizedById { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? ExecutedById { get; set; }
    public decimal? AppraisedValueAtRetirement { get; set; }
    public string? Observations { get; set; }
    public string? RejectionReason { get; set; }

    public Resource Resource { get; set; } = null!;
}
