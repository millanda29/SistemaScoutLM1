using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Movement : BaseEntity
{
    public int ResourceId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    public int? PreviousLocationId { get; set; }
    public int? NewLocationId { get; set; }
    public string? PreviousResponsibleId { get; set; }
    public string? NewResponsibleId { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string PerformedById { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string? Observations { get; set; }

    public Resource Resource { get; set; } = null!;
    public Location? PreviousLocation { get; set; }
    public Location? NewLocation { get; set; }
}
