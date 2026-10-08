using ScoutAsset.Server.Application.Models.Maintenance;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface IMaintenanceService
{
    Task<List<Maintenance>> GetAllAsync(string? status, int? resourceId);
    Task<Maintenance?> GetByIdAsync(int id);
    Task<Maintenance> CreateAsync(CreateMaintenanceRequest request, string currentUserId);
    Task StartAsync(int id, string currentUserId);
    Task CompleteAsync(int id, CompleteMaintenanceRequest request);
    Task<List<Maintenance>> GetOverdueAsync();
    Task<int> AutoScheduleAsync(int intervalMonths, string currentUserId);
}
