namespace ScoutAsset.Server.Application.Models.Maintenance;

public record CreateMaintenanceRequest(
    int ResourceId,
    string Type,
    string Description,
    DateTime ScheduledDate,
    string? ResponsibleId,
    decimal? Cost,
    string? Observations,
    DateTime? NextMaintenanceDate);
