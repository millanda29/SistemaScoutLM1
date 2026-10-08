using ScoutAsset.Server.Application.Models.Dashboard;

namespace ScoutAsset.Server.Application.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync();
}
