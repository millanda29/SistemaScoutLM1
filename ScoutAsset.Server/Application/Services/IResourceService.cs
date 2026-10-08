using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface IResourceService
{
    Task<List<Resource>> GetAllAsync(string? search, int? categoryId, int? locationId, string? status);
    Task<Resource?> GetByIdAsync(int id);
    Task<Resource> CreateAsync(CreateResourceRequest request, string currentUserId);
    Task UpdateAsync(int id, UpdateResourceRequest request);
    Task ChangeLocationAsync(int id, ChangeLocationRequest request, string currentUserId);
    Task ChangeResponsibleAsync(int id, ChangeResponsibleRequest request, string currentUserId);
    Task<List<Movement>> GetMovementsAsync(int id);
    Task DeleteAsync(int id);
    Task<ResourceImportResult> ImportResourcesAsync(Stream stream, string fileName, string currentUserId);
}
