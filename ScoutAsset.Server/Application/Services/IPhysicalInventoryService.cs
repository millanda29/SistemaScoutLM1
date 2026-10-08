using ScoutAsset.Server.Application.Models.PhysicalInventory;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface IPhysicalInventoryService
{
    Task<List<PhysicalInventory>> GetAllAsync();
    Task<PhysicalInventory?> GetByIdAsync(int id);
    Task<PhysicalInventory> CreateAsync(CreateInventoryRequest request, string currentUserId);
    Task StartAsync(int id);
    Task<InventoryItem> RegisterItemAsync(int id, RegisterItemRequest request, string currentUserId);
    Task FinishAsync(int id);
    Task<InventoryReconciliation> ReconcileAsync(int id, string currentUserId);
    Task<InventoryReconciliation?> GetReconciliationAsync(int id);
}
