using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ReportsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GeneralInventory()
    {
        var resources = await _context.Resources
            .Include(r => r.Category)
            .Include(r => r.Location)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Code)
            .Select(r => new
            {
                r.Code,
                r.Name,
                Category = r.Category.Name,
                Location = r.Location.Name,
                r.PhysicalCondition,
                r.AdministrativeStatus,
                r.CurrentResponsibleId,
                r.AcquisitionType,
                r.AcquisitionCost,
                r.AppraisedValue,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(resources);
    }

    [HttpGet("by-category")]
    public async Task<IActionResult> ByCategory()
    {
        var result = await _context.Categories
            .Where(c => c.IsActive)
            .Select(c => new
            {
                Category = c.Name,
                Total = c.Resources.Count(r => r.IsActive),
                Disponibles = c.Resources.Count(r => r.IsActive && r.AdministrativeStatus == nameof(AdministrativeStatus.DISPONIBLE)),
                Prestados = c.Resources.Count(r => r.IsActive && r.AdministrativeStatus == nameof(AdministrativeStatus.PRESTADO)),
                EnMantenimiento = c.Resources.Count(r => r.IsActive && r.AdministrativeStatus == nameof(AdministrativeStatus.EN_MANTENIMIENTO))
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("by-location")]
    public async Task<IActionResult> ByLocation()
    {
        var result = await _context.Locations
            .Where(l => l.IsActive)
            .Select(l => new
            {
                Location = l.Name,
                Total = l.Resources.Count(r => r.IsActive)
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("by-status")]
    public async Task<IActionResult> ByStatus()
    {
        var statuses = new[]
        {
            nameof(AdministrativeStatus.DISPONIBLE),
            nameof(AdministrativeStatus.ASIGNADO),
            nameof(AdministrativeStatus.PRESTADO),
            nameof(AdministrativeStatus.EN_MANTENIMIENTO),
            nameof(AdministrativeStatus.NO_LOCALIZADO),
            nameof(AdministrativeStatus.PERDIDO),
            nameof(AdministrativeStatus.DADO_DE_BAJA)
        };

        var result = new List<object>();
        foreach (var status in statuses)
        {
            var count = await _context.Resources
                .CountAsync(r => r.IsActive && r.AdministrativeStatus == status);
            result.Add(new { Status = status, Total = count });
        }

        return Ok(result);
    }

    [HttpGet("active-loans")]
    public async Task<IActionResult> ActiveLoans()
    {
        var dirigentes = await _context.Dirigentes.ToDictionaryAsync(d => d.Id.ToString(), d => $"{d.Nombres} {d.Apellidos}");
        var loans = await _context.Loans
            .Include(l => l.Items)
            .ThenInclude(li => li.Resource)
            .Where(l => l.Status == nameof(LoanStatus.PRESTADO) || l.Status == nameof(LoanStatus.ENTREGADO))
            .OrderBy(l => l.ExpectedReturnDate)
            .ToListAsync();

        var result = loans.Select(l => new
        {
            l.RequestNumber,
            l.RequesterId,
            RequesterName = dirigentes.TryGetValue(l.RequesterId, out var name) ? name : l.RequesterId,
            l.Reason,
            l.ExpectedReturnDate,
            l.ActualDeliveryDate,
            DaysOverdue = l.ExpectedReturnDate < DateTime.UtcNow ? EF.Functions.DateDiffDay(l.ExpectedReturnDate, DateTime.UtcNow) : 0,
            ResourceCount = l.Items.Count,
            ResourceNames = string.Join(", ", l.Items.Select(li => li.Resource != null ? $"{li.Resource.Code} - {li.Resource.Name}" : ""))
        });

        return Ok(result);
    }

    [HttpGet("overdue-loans")]
    public async Task<IActionResult> OverdueLoans()
    {
        var dirigentes = await _context.Dirigentes.ToDictionaryAsync(d => d.Id.ToString(), d => $"{d.Nombres} {d.Apellidos}");
        var loans = await _context.Loans
            .Include(l => l.Items)
            .ThenInclude(li => li.Resource)
            .Where(l => (l.Status == nameof(LoanStatus.PRESTADO) || l.Status == nameof(LoanStatus.ENTREGADO))
                && l.ExpectedReturnDate < DateTime.UtcNow)
            .OrderBy(l => l.ExpectedReturnDate)
            .ToListAsync();

        var result = loans.Select(l => new
        {
            l.RequestNumber,
            l.RequesterId,
            RequesterName = dirigentes.TryGetValue(l.RequesterId, out var name) ? name : l.RequesterId,
            l.Reason,
            l.ExpectedReturnDate,
            l.ActualDeliveryDate,
            DaysOverdue = EF.Functions.DateDiffDay(l.ExpectedReturnDate, DateTime.UtcNow),
            ResourceCount = l.Items.Count,
            ResourceNames = string.Join(", ", l.Items.Select(li => li.Resource != null ? $"{li.Resource.Code} - {li.Resource.Name}" : ""))
        });

        return Ok(result);
    }

    [HttpGet("pending-maintenance")]
    public async Task<IActionResult> PendingMaintenance()
    {
        var maintenances = await _context.Maintenances
            .Include(m => m.Resource)
            .Where(m => m.Status == nameof(MaintenanceStatus.PROGRAMADO)
                || m.Status == nameof(MaintenanceStatus.PENDIENTE)
                || m.Status == nameof(MaintenanceStatus.EN_PROCESO))
            .OrderBy(m => m.ScheduledDate)
            .Select(m => new
            {
                ResourceCode = m.Resource.Code,
                ResourceName = m.Resource.Name,
                m.Type,
                m.Status,
                m.ScheduledDate,
                m.Description
            })
            .ToListAsync();

        return Ok(maintenances);
    }

    [HttpGet("movements")]
    public async Task<IActionResult> Movements(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? type)
    {
        var query = _context.Movements
            .Include(m => m.Resource)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(m => m.PerformedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(m => m.PerformedAt <= to.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(m => m.Type == type);

        var result = await query
            .OrderByDescending(m => m.PerformedAt)
            .Select(m => new
            {
                ResourceCode = m.Resource.Code,
                ResourceName = m.Resource.Name,
                m.Type,
                m.Description,
                m.PreviousStatus,
                m.NewStatus,
                m.PerformedAt,
                m.PerformedById
            })
            .Take(500)
            .ToListAsync();

        return Ok(result);
    }
}
