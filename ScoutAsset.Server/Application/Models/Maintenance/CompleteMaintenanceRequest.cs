namespace ScoutAsset.Server.Application.Models.Maintenance;

public record CompleteMaintenanceRequest(
    string? Result,
    decimal? Cost,
    DateTime? NextMaintenanceDate,
    string? Observations);
