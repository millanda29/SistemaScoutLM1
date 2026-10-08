using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class ResourceAssignment : BaseEntity
{
    public int ResourceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AssignedById { get; set; } = string.Empty;
    public string AssignmentType { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Observations { get; set; }

    public Resource Resource { get; set; } = null!;
}
