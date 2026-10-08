using ScoutAsset.Server.Domain.Common;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Domain.Entities;

public class PhysicalInventoryLocation : BaseEntity
{
    public int PhysicalInventoryId { get; set; }
    public int LocationId { get; set; }
    public string Status { get; set; } = nameof(InventoryLocationStatus.PENDIENTE);

    public PhysicalInventory PhysicalInventory { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
