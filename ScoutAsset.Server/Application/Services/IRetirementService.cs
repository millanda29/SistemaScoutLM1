using ScoutAsset.Server.Application.Models.Retirements;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface IRetirementService
{
    Task<List<Retirement>> GetAllAsync(string? status);
    Task<Retirement?> GetByIdAsync(int id);
    Task<Retirement> CreateRequestAsync(RequestRetirementRequest request, string currentUserId);
    Task ReviewAsync(int id, string currentUserId);
    Task AuthorizeAsync(int id, string currentUserId);
    Task RejectAsync(int id, RejectRetirementRequest request);
    Task ExecuteAsync(int id, string currentUserId);
}
