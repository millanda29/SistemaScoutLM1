namespace ScoutAsset.Server.Application.Models.Resources;

public record CreateResourceRequest(
    string Name,
    string? Description,
    int CategoryId,
    string? Brand,
    string? Model,
    string? SerialNumber,
    DateTime? AcquisitionDate,
    string? AcquisitionType,
    decimal? AcquisitionCost,
    decimal? AppraisedValue,
    string? PhysicalCondition,
    int LocationId,
    string? Observations,
    string? CurrentResponsibleId = null);
