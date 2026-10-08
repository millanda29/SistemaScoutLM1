namespace ScoutAsset.Server.Application.Models.Resources;

public record UpdateResourceRequest(
    string Name,
    string? Description,
    int CategoryId,
    int LocationId,
    string? Brand,
    string? Model,
    string? SerialNumber,
    DateTime? AcquisitionDate,
    string? AcquisitionType,
    decimal? AcquisitionCost,
    decimal? AppraisedValue,
    string? PhysicalCondition,
    string? Observations,
    string? CurrentResponsibleId = null);
