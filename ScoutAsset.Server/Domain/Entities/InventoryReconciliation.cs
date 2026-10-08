using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class InventoryReconciliation : BaseEntity
{
    public int PhysicalInventoryId { get; set; }
    public int TotalRegistered { get; set; }
    public int TotalFound { get; set; }
    public int TotalNotFound { get; set; }
    public int TotalDamaged { get; set; }
    public int TotalNew { get; set; }
    public int TotalNotIdentified { get; set; }
    public DateTime ReconciledAt { get; set; } = DateTime.UtcNow;
    public string ReconciledById { get; set; } = string.Empty;
    public string? Observations { get; set; }

    public PhysicalInventory PhysicalInventory { get; set; } = null!;
}
