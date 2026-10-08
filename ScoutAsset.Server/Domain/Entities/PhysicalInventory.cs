using ScoutAsset.Server.Domain.Common;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Domain.Entities;

public class PhysicalInventory : BaseEntity
{
    public string InventoryNumber { get; set; } = string.Empty;
    public string Status { get; set; } = nameof(InventoryStatus.PLANIFICADA);
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ResponsibleId { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public string CreatedById { get; set; } = string.Empty;

    public ICollection<PhysicalInventoryLocation> Locations { get; set; } = new List<PhysicalInventoryLocation>();
    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
    public InventoryReconciliation? Reconciliation { get; set; }
}
