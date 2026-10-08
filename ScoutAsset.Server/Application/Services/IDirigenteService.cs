using ScoutAsset.Server.Application.Models.Dirigentes;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface IDirigenteService
{
    Task<List<Dirigente>> GetAllAsync();
    Task<Dirigente?> GetByIdAsync(int id);
    Task<Dirigente> CreateAsync(CreateDirigenteRequest request);
    Task UpdateAsync(int id, CreateDirigenteRequest request);
    Task DeleteAsync(int id);
    Task<ResourceImportResult> ImportDirigentesAsync(Stream stream, string fileName);
}
