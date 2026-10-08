using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public int PhysicalInventoryId { get; set; }
    public int? ResourceId { get; set; }
    public int LocationId { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? PhysicalCondition { get; set; }
    public string? FoundCode { get; set; }
    public string? FoundName { get; set; }
    public string? Observations { get; set; }
    public string VerifiedById { get; set; } = string.Empty;
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

    public PhysicalInventory PhysicalInventory { get; set; } = null!;
    public Resource? Resource { get; set; }
    public Location Location { get; set; } = null!;
}
