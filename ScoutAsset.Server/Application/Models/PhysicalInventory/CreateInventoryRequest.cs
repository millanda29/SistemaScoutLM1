namespace ScoutAsset.Server.Application.Models.PhysicalInventory;

public record CreateInventoryRequest(string? Description, DateTime StartDate, List<int> LocationIds);
