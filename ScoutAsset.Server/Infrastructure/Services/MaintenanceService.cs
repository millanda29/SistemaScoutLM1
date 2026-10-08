using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Maintenance;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IApplicationDbContext _context;

    public MaintenanceService(IApplicationDbContext context) => _context = context;

    public async Task<List<Maintenance>> GetAllAsync(string? status, int? resourceId)
    {
        var query = _context.Maintenances.Include(m => m.Resource).AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(m => m.Status == status);
        if (resourceId.HasValue)
            query = query.Where(m => m.ResourceId == resourceId);
        return await query.OrderByDescending(m => m.ScheduledDate).ToListAsync();
    }

    public async Task<Maintenance?> GetByIdAsync(int id) =>
        await _context.Maintenances.Include(m => m.Resource).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Maintenance> CreateAsync(CreateMaintenanceRequest request, string currentUserId)
    {
        var resource = await _context.Resources.FindAsync(request.ResourceId)
            ?? throw new InvalidOperationException("Recurso no encontrado");

        var maintenance = new Maintenance
        {
            ResourceId = request.ResourceId,
            Type = request.Type,
            Status = nameof(MaintenanceStatus.PROGRAMADO),
            Description = request.Description,
            ScheduledDate = request.ScheduledDate,
            ResponsibleId = request.ResponsibleId,
            Cost = request.Cost,
            Observations = request.Observations,
            NextMaintenanceDate = request.NextMaintenanceDate,
            CreatedById = currentUserId
        };

        _context.Maintenances.Add(maintenance);
        await _context.SaveChangesAsync();
        return maintenance;
    }

    public async Task StartAsync(int id, string currentUserId)
    {
        var maintenance = await _context.Maintenances.FindAsync(id)
            ?? throw new KeyNotFoundException("Mantenimiento no encontrado");

        maintenance.Status = nameof(MaintenanceStatus.EN_PROCESO);

        var resource = await _context.Resources.FindAsync(maintenance.ResourceId);
        if (resource != null && resource.AdministrativeStatus == nameof(AdministrativeStatus.DISPONIBLE))
        {
            resource.AdministrativeStatus = nameof(AdministrativeStatus.EN_MANTENIMIENTO);
            resource.UpdatedAt = DateTime.UtcNow;
            _context.Movements.Add(new Movement
            {
                ResourceId = resource.Id,
                Type = nameof(MovementType.MANTENIMIENTO),
                Description = "Inicio de mantenimiento",
                PreviousStatus = nameof(AdministrativeStatus.DISPONIBLE),
                NewStatus = nameof(AdministrativeStatus.EN_MANTENIMIENTO),
                ReferenceType = "MAINTENANCE",
                ReferenceId = maintenance.Id,
                PerformedById = currentUserId,
                PerformedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task CompleteAsync(int id, CompleteMaintenanceRequest request)
    {
        var maintenance = await _context.Maintenances.FindAsync(id)
            ?? throw new KeyNotFoundException("Mantenimiento no encontrado");

        maintenance.Status = nameof(MaintenanceStatus.COMPLETADO);
        maintenance.CompletedDate = DateTime.UtcNow;
        maintenance.Result = request.Result;
        maintenance.Cost = request.Cost;
        maintenance.NextMaintenanceDate = request.NextMaintenanceDate;
        maintenance.Observations = request.Observations;

        var resource = await _context.Resources.FindAsync(maintenance.ResourceId);
        if (resource != null)
        {
            resource.AdministrativeStatus = nameof(AdministrativeStatus.DISPONIBLE);
            resource.LastMaintenanceDate = DateTime.UtcNow;
            resource.NextMaintenanceDate = request.NextMaintenanceDate;
            resource.PhysicalCondition = nameof(PhysicalCondition.BUENO);
            resource.UpdatedAt = DateTime.UtcNow;
            _context.Movements.Add(new Movement
            {
                ResourceId = resource.Id,
                Type = nameof(MovementType.MANTENIMIENTO),
                Description = "Mantenimiento completado",
                PreviousStatus = nameof(AdministrativeStatus.EN_MANTENIMIENTO),
                NewStatus = nameof(AdministrativeStatus.DISPONIBLE),
                ReferenceType = "MAINTENANCE",
                ReferenceId = maintenance.Id,
                PerformedById = "system",
                PerformedAt = DateTime.UtcNow
            });
        }

        // Auto-schedule next preventive maintenance if next maintenance date is provided
        if (request.NextMaintenanceDate.HasValue)
        {
            var nextMaint = new Maintenance
            {
                ResourceId = maintenance.ResourceId,
                Type = nameof(MaintenanceType.PREVENTIVO),
                Status = nameof(MaintenanceStatus.PROGRAMADO),
                Description = $"Mantenimiento preventivo periódico para {resource?.Name ?? "Recurso"}",
                ScheduledDate = request.NextMaintenanceDate.Value,
                CreatedById = maintenance.CreatedById
            };
            _context.Maintenances.Add(nextMaint);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Maintenance>> GetOverdueAsync() =>
        await _context.Maintenances
            .Include(m => m.Resource)
            .Where(m => (m.Status == nameof(MaintenanceStatus.PROGRAMADO) || m.Status == nameof(MaintenanceStatus.PENDIENTE))
                && m.ScheduledDate < DateTime.UtcNow)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync();

    public async Task<int> AutoScheduleAsync(int intervalMonths, string currentUserId)
    {
        if (intervalMonths <= 0) intervalMonths = 6;

        var activeResources = await _context.Resources
            .Where(r => r.IsActive)
            .ToListAsync();

        var activeMaintenances = await _context.Maintenances
            .Where(m => m.Status == nameof(MaintenanceStatus.PROGRAMADO) || m.Status == nameof(MaintenanceStatus.EN_PROCESO))
            .Select(m => m.ResourceId)
            .ToListAsync();

        int scheduledCount = 0;
        foreach (var res in activeResources)
        {
            if (activeMaintenances.Contains(res.Id)) continue;

            DateTime targetDate;
            if (res.NextMaintenanceDate.HasValue && res.NextMaintenanceDate > DateTime.UtcNow)
            {
                targetDate = res.NextMaintenanceDate.Value;
            }
            else
            {
                var baseDate = res.LastMaintenanceDate ?? res.CreatedAt;
                targetDate = baseDate.AddMonths(intervalMonths);
                if (targetDate < DateTime.UtcNow)
                    targetDate = DateTime.UtcNow.AddDays(7);
            }

            var newMaint = new Maintenance
            {
                ResourceId = res.Id,
                Type = nameof(MaintenanceType.PREVENTIVO),
                Status = nameof(MaintenanceStatus.PROGRAMADO),
                Description = $"Mantenimiento preventivo automático ({res.Code} - {res.Name})",
                ScheduledDate = targetDate,
                CreatedById = currentUserId
            };

            res.NextMaintenanceDate = targetDate;
            _context.Maintenances.Add(newMaint);
            scheduledCount++;
        }

        if (scheduledCount > 0)
            await _context.SaveChangesAsync();

        return scheduledCount;
    }
}
