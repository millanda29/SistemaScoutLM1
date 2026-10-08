using ScoutAsset.Server.Application.Models.Losses;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface ILossService
{
    Task<List<Loss>> GetAllAsync(string? status);
    Task<Loss?> GetByIdAsync(int id);
    Task<Loss> ReportAsync(ReportLossRequest request, string currentUserId);
    Task ConfirmAsync(int id, string currentUserId);
    Task RecoverAsync(int id);
    Task InvestigateAsync(int id, InvestigateRequest request);
}
