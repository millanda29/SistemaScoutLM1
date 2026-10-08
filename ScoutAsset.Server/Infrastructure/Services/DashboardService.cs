using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Dashboard;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;

    public DashboardService(IApplicationDbContext context) => _context = context;

    public async Task<DashboardResponse> GetAsync()
    {
        var resourceStats = await _context.Resources
            .Where(r => r.IsActive)
            .GroupBy(r => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Disponibles = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.DISPONIBLE)),
                Prestados = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.PRESTADO)),
                EnMantenimiento = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.EN_MANTENIMIENTO)),
                NoLocalizados = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.NO_LOCALIZADO)),
                Perdidos = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.PERDIDO)),
                DadosDeBaja = g.Count(r => r.AdministrativeStatus == nameof(AdministrativeStatus.DADO_DE_BAJA)),
                Daniados = g.Count(r => r.PhysicalCondition == nameof(PhysicalCondition.DANIADO)),
                TotalCost = g.Sum(r => r.AcquisitionCost ?? 0),
                TotalAppraised = g.Sum(r => r.AppraisedValue ?? 0)
            })
            .FirstOrDefaultAsync();

        var prestamosVencidos = await _context.Loans
            .CountAsync(l => (l.Status == nameof(LoanStatus.PRESTADO) || l.Status == nameof(LoanStatus.ENTREGADO)) && l.ExpectedReturnDate < DateTime.UtcNow);

        var categoryStats = await _context.Categories
            .Where(c => c.IsActive)
            .Select(c => new
            {
                Category = c.Name,
                Count = c.Resources.Count(r => r.IsActive),
                Disponibles = c.Resources.Count(r => r.IsActive && r.AdministrativeStatus == nameof(AdministrativeStatus.DISPONIBLE))
            })
            .OrderByDescending(c => c.Count)
            .Take(6)
            .Select(c => new CategoryStatDto(c.Category, c.Count, c.Disponibles))
            .ToListAsync();

        var recentMovements = await _context.Movements
            .Include(m => m.Resource)
            .OrderByDescending(m => m.PerformedAt)
            .Take(6)
            .Select(m => new RecentMovementDto(
                m.Resource.Code,
                m.Resource.Name,
                m.Type,
                m.Description,
                m.PerformedAt
            ))
            .ToListAsync();

        var total = resourceStats?.Total ?? 0;
        var disponibles = resourceStats?.Disponibles ?? 0;
        var prestados = resourceStats?.Prestados ?? 0;
        var enMantenimiento = resourceStats?.EnMantenimiento ?? 0;
        var noLocalizados = resourceStats?.NoLocalizados ?? 0;
        var perdidos = resourceStats?.Perdidos ?? 0;
        var dadosDeBaja = resourceStats?.DadosDeBaja ?? 0;
        var daniados = resourceStats?.Daniados ?? 0;
        var totalCost = resourceStats?.TotalCost ?? 0;
        var totalAppraised = resourceStats?.TotalAppraised ?? 0;

        return new DashboardResponse(
            total, disponibles, prestados, enMantenimiento,
            noLocalizados, perdidos, dadosDeBaja, daniados, prestamosVencidos,
            totalCost, totalAppraised, categoryStats, recentMovements);
    }
}
