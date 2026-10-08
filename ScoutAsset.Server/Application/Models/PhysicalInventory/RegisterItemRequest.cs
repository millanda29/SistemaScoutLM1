namespace ScoutAsset.Server.Application.Models.PhysicalInventory;

public record RegisterItemRequest(
    int? ResourceId,
    int LocationId,
    string Result,
    string? PhysicalCondition,
    string? FoundCode,
    string? FoundName,
    string? Observations);
