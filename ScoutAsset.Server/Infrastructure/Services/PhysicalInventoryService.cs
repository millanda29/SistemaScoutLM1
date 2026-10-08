using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.PhysicalInventory;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class PhysicalInventoryService : IPhysicalInventoryService
{
    private readonly IApplicationDbContext _context;

    public PhysicalInventoryService(IApplicationDbContext context) => _context = context;

    public async Task<List<PhysicalInventory>> GetAllAsync() =>
        await _context.PhysicalInventories
            .Include(i => i.Locations)
            .ThenInclude(il => il.Location)
            .Include(i => i.Items)
            .OrderByDescending(i => i.StartDate)
            .ToListAsync();

    public async Task<PhysicalInventory?> GetByIdAsync(int id) =>
        await _context.PhysicalInventories
            .Include(i => i.Locations)
            .ThenInclude(il => il.Location)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<PhysicalInventory> CreateAsync(CreateInventoryRequest request, string currentUserId)
    {
        var date = DateTime.UtcNow;
        var inventoryNumber = $"INV-{date:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";

        var inventory = new PhysicalInventory
        {
            InventoryNumber = inventoryNumber,
            Status = nameof(InventoryStatus.PLANIFICADA),
            Description = request.Description,
            StartDate = request.StartDate,
            ResponsibleId = currentUserId,
            CreatedById = currentUserId
        };

        foreach (var locationId in request.LocationIds)
            inventory.Locations.Add(new PhysicalInventoryLocation { LocationId = locationId });

        _context.PhysicalInventories.Add(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    public async Task StartAsync(int id)
    {
        var inventory = await _context.PhysicalInventories.FindAsync(id)
            ?? throw new KeyNotFoundException("Inventario no encontrado");
        inventory.Status = nameof(InventoryStatus.EN_CURSO);
        await _context.SaveChangesAsync();
    }

    public async Task<InventoryItem> RegisterItemAsync(int id, RegisterItemRequest request, string currentUserId)
    {
        var inventory = await _context.PhysicalInventories
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Inventario no encontrado");
        if (inventory.Status != nameof(InventoryStatus.EN_CURSO))
            throw new InvalidOperationException("El inventario debe estar EN_CURSO");

        string? foundCode = request.FoundCode;
        string? foundName = request.FoundName;

        if (request.ResourceId.HasValue)
        {
            var res = await _context.Resources.FindAsync(request.ResourceId.Value);
            if (res != null)
            {
                foundCode ??= res.Code;
                foundName ??= res.Name;
            }
        }

        var item = new InventoryItem
        {
            PhysicalInventoryId = id,
            ResourceId = request.ResourceId,
            LocationId = request.LocationId,
            Result = request.Result,
            PhysicalCondition = request.PhysicalCondition,
            FoundCode = foundCode,
            FoundName = foundName,
            Observations = request.Observations,
            VerifiedById = currentUserId,
            VerifiedAt = DateTime.UtcNow
        };

        _context.InventoryItems.Add(item);

        if (request.Result == nameof(InventoryResult.NO_LOCALIZADO) && request.ResourceId.HasValue)
        {
            var resource = await _context.Resources.FindAsync(request.ResourceId.Value);
            if (resource != null)
            {
                resource.AdministrativeStatus = nameof(AdministrativeStatus.NO_LOCALIZADO);
                resource.UpdatedAt = DateTime.UtcNow;
                _context.Movements.Add(new Movement
                {
                    ResourceId = resource.Id,
                    Type = nameof(MovementType.PERDIDA),
                    Description = $"No localizado en inventario {inventory.InventoryNumber}",
                    PreviousStatus = resource.AdministrativeStatus,
                    NewStatus = nameof(AdministrativeStatus.NO_LOCALIZADO),
                    ReferenceType = "PHYSICAL_INVENTORY",
                    ReferenceId = inventory.Id,
                    PerformedById = currentUserId,
                    PerformedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
        return item;
    }

    public async Task FinishAsync(int id)
    {
        var inventory = await _context.PhysicalInventories
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Inventario no encontrado");

        inventory.Status = nameof(InventoryStatus.FINALIZADA);
        inventory.EndDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<InventoryReconciliation> ReconcileAsync(int id, string currentUserId)
    {
        var inventory = await _context.PhysicalInventories
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Inventario no encontrado");
        if (inventory.Status != nameof(InventoryStatus.FINALIZADA))
            throw new InvalidOperationException("El inventario debe estar FINALIZADA para conciliar");

        var totalRegistered = await _context.Resources.CountAsync(r => r.IsActive);

        var reconciliation = new InventoryReconciliation
        {
            PhysicalInventoryId = id,
            TotalRegistered = totalRegistered,
            TotalFound = inventory.Items.Count(i => i.Result == nameof(InventoryResult.ENCONTRADO)),
            TotalNotFound = inventory.Items.Count(i => i.Result == nameof(InventoryResult.NO_LOCALIZADO)),
            TotalDamaged = inventory.Items.Count(i => i.Result == nameof(InventoryResult.DANIADO)),
            TotalNew = inventory.Items.Count(i => i.Result == nameof(InventoryResult.NUEVO)),
            TotalNotIdentified = inventory.Items.Count(i => i.Result == nameof(InventoryResult.NO_IDENTIFICADO)),
            ReconciledById = currentUserId,
            ReconciledAt = DateTime.UtcNow
        };

        _context.InventoryReconciliations.Add(reconciliation);
        inventory.Status = nameof(InventoryStatus.CONCILIADA);
        await _context.SaveChangesAsync();
        return reconciliation;
    }

    public async Task<InventoryReconciliation?> GetReconciliationAsync(int id) =>
        await _context.InventoryReconciliations
            .FirstOrDefaultAsync(r => r.PhysicalInventoryId == id);
}
