namespace ScoutAsset.Server.Application.Models.Loans;

public record ReturnLoanRequest(string ConditionAtReturn, string? Observations, string? DamagesDetected);
