using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Losses;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class LossService : ILossService
{
    private readonly IApplicationDbContext _context;

    public LossService(IApplicationDbContext context) => _context = context;

    public async Task<List<Loss>> GetAllAsync(string? status)
    {
        var query = _context.Losses.Include(l => l.Resource).AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);
        return await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
    }

    public async Task<Loss?> GetByIdAsync(int id) =>
        await _context.Losses.Include(l => l.Resource).FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loss> ReportAsync(ReportLossRequest request, string currentUserId)
    {
        var resource = await _context.Resources.FindAsync(request.ResourceId)
            ?? throw new InvalidOperationException("Recurso no encontrado");

        if (resource.AdministrativeStatus != nameof(AdministrativeStatus.NO_LOCALIZADO))
            throw new InvalidOperationException("El recurso debe estar NO_LOCALIZADO");

        var date = DateTime.UtcNow;
        var lossNumber = $"PERD-{date:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";

        var loss = new Loss
        {
            ResourceId = request.ResourceId,
            LossNumber = lossNumber,
            Status = nameof(LossStatus.EN_INVESTIGACION),
            Circumstances = request.Circumstances,
            Description = request.Description,
            ReportedById = currentUserId
        };

        _context.Losses.Add(loss);
        await _context.SaveChangesAsync();
        return loss;
    }

    public async Task ConfirmAsync(int id, string currentUserId)
    {
        var loss = await _context.Losses
            .Include(l => l.Resource)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new KeyNotFoundException("Pérdida no encontrada");

        if (loss.Status != nameof(LossStatus.EN_INVESTIGACION))
            throw new InvalidOperationException("La pérdida debe estar EN_INVESTIGACION");

        loss.Status = nameof(LossStatus.CONFIRMADA);
        loss.ConfirmedById = currentUserId;
        loss.ConfirmedAt = DateTime.UtcNow;

        if (loss.Resource != null)
        {
            loss.Resource.AdministrativeStatus = nameof(AdministrativeStatus.PERDIDO);
            loss.Resource.UpdatedAt = DateTime.UtcNow;
            _context.Movements.Add(new Movement
            {
                ResourceId = loss.ResourceId,
                Type = nameof(MovementType.PERDIDA),
                Description = $"Pérdida confirmada #{loss.LossNumber}",
                PreviousStatus = nameof(AdministrativeStatus.NO_LOCALIZADO),
                NewStatus = nameof(AdministrativeStatus.PERDIDO),
                ReferenceType = "LOSS",
                ReferenceId = loss.Id,
                PerformedById = currentUserId,
                PerformedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RecoverAsync(int id)
    {
        var loss = await _context.Losses
            .Include(l => l.Resource)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new KeyNotFoundException("Pérdida no encontrada");

        loss.Status = nameof(LossStatus.RECUPERADA);
        loss.RecoveredAt = DateTime.UtcNow;

        if (loss.Resource != null)
        {
            loss.Resource.AdministrativeStatus = nameof(AdministrativeStatus.DISPONIBLE);
            loss.Resource.UpdatedAt = DateTime.UtcNow;
            _context.Movements.Add(new Movement
            {
                ResourceId = loss.ResourceId,
                Type = nameof(MovementType.RECUPERACION),
                Description = $"Recuperación #{loss.LossNumber}",
                PreviousStatus = nameof(AdministrativeStatus.PERDIDO),
                NewStatus = nameof(AdministrativeStatus.DISPONIBLE),
                ReferenceType = "LOSS",
                ReferenceId = loss.Id,
                PerformedById = "system",
                PerformedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task InvestigateAsync(int id, InvestigateRequest request)
    {
        var loss = await _context.Losses.FindAsync(id)
            ?? throw new KeyNotFoundException("Pérdida no encontrada");
        loss.Observations = request.Observations;
        loss.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
