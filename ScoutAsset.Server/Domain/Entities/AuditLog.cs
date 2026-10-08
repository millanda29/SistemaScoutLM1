namespace ScoutAsset.Server.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? PreviousValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangedFields { get; set; }
    public string? IpAddress { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string? Observations { get; set; }
}
