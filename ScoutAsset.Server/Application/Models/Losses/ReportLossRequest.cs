namespace ScoutAsset.Server.Application.Models.Losses;

public record ReportLossRequest(int ResourceId, string Circumstances, string? Description);
