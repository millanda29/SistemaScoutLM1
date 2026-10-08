using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Maintenance : BaseEntity
{
    public int ResourceId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? ResponsibleId { get; set; }
    public decimal? Cost { get; set; }
    public string? Result { get; set; }
    public string? Observations { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string CreatedById { get; set; } = string.Empty;

    public Resource Resource { get; set; } = null!;
}
