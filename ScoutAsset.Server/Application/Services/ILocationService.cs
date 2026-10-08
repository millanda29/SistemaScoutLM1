using ScoutAsset.Server.Application.Models.Locations;
using ScoutAsset.Server.Domain.Entities;

using ScoutAsset.Server.Application.Models.Resources;

namespace ScoutAsset.Server.Application.Services;

public interface ILocationService
{
    Task<List<Location>> GetAllAsync();
    Task<Location?> GetByIdAsync(int id);
    Task<Location> CreateAsync(CreateLocationRequest request);
    Task UpdateAsync(int id, CreateLocationRequest request);
    Task DeleteAsync(int id);
    Task<ResourceImportResult> ImportLocationsAsync(Stream stream, string fileName);
}
