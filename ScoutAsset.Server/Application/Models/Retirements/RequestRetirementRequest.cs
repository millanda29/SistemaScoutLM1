namespace ScoutAsset.Server.Application.Models.Retirements;

public record RequestRetirementRequest(int ResourceId, string Reason, decimal? AppraisedValue, string? Observations);
