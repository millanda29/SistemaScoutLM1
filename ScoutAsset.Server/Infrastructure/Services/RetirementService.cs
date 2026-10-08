using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Retirements;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class RetirementService : IRetirementService
{
    private readonly IApplicationDbContext _context;

    public RetirementService(IApplicationDbContext context) => _context = context;

    public async Task<List<Retirement>> GetAllAsync(string? status)
    {
        var query = _context.Retirements.Include(r => r.Resource).AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);
        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<Retirement?> GetByIdAsync(int id) =>
        await _context.Retirements.Include(r => r.Resource).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Retirement> CreateRequestAsync(RequestRetirementRequest request, string currentUserId)
    {
        var resource = await _context.Resources.FindAsync(request.ResourceId)
            ?? throw new InvalidOperationException("Recurso no encontrado");

        var hasActiveRetirement = await _context.Retirements.AnyAsync(r =>
            r.ResourceId == request.ResourceId
            && r.Status != nameof(RetirementStatus.RECHAZADA)
            && r.Status != nameof(RetirementStatus.EJECUTADA));

        if (hasActiveRetirement)
            throw new InvalidOperationException("El recurso ya tiene una solicitud de baja activa");

        var date = DateTime.UtcNow;
        var retirementNumber = $"BAJA-{date:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";

        var retirement = new Retirement
        {
            ResourceId = request.ResourceId,
            RetirementNumber = retirementNumber,
            Status = nameof(RetirementStatus.SOLICITADA),
            Reason = request.Reason,
            RequestedById = currentUserId,
            AppraisedValueAtRetirement = request.AppraisedValue,
            Observations = request.Observations
        };

        _context.Retirements.Add(retirement);
        await _context.SaveChangesAsync();
        return retirement;
    }

    public async Task ReviewAsync(int id, string currentUserId)
    {
        var retirement = await _context.Retirements.FindAsync(id)
            ?? throw new KeyNotFoundException("Baja no encontrada");
        if (retirement.Status != nameof(RetirementStatus.SOLICITADA))
            throw new InvalidOperationException("La baja debe estar SOLICITADA");

        retirement.Status = nameof(RetirementStatus.REVISADA);
        retirement.ReviewedById = currentUserId;
        await _context.SaveChangesAsync();
    }

    public async Task AuthorizeAsync(int id, string currentUserId)
    {
        var retirement = await _context.Retirements
            .Include(r => r.Resource)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Baja no encontrada");
        if (retirement.Status != nameof(RetirementStatus.REVISADA))
            throw new InvalidOperationException("La baja debe estar REVISADA");

        retirement.Status = nameof(RetirementStatus.AUTORIZADA);
        retirement.AuthorizedById = currentUserId;
        retirement.AuthorizedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, RejectRetirementRequest request)
    {
        var retirement = await _context.Retirements.FindAsync(id)
            ?? throw new KeyNotFoundException("Baja no encontrada");
        retirement.Status = nameof(RetirementStatus.RECHAZADA);
        retirement.RejectionReason = request.Reason;
        await _context.SaveChangesAsync();
    }

    public async Task ExecuteAsync(int id, string currentUserId)
    {
        var retirement = await _context.Retirements
            .Include(r => r.Resource)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Baja no encontrada");
        if (retirement.Status != nameof(RetirementStatus.AUTORIZADA))
            throw new InvalidOperationException("La baja debe estar AUTORIZADA");

        retirement.Status = nameof(RetirementStatus.EJECUTADA);
        retirement.ExecutedById = currentUserId;
        retirement.ExecutedAt = DateTime.UtcNow;

        if (retirement.Resource != null)
        {
            retirement.Resource.AdministrativeStatus = nameof(AdministrativeStatus.DADO_DE_BAJA);
            retirement.Resource.UpdatedAt = DateTime.UtcNow;
            _context.Movements.Add(new Movement
            {
                ResourceId = retirement.ResourceId,
                Type = nameof(MovementType.BAJA),
                Description = $"Baja ejecutada #{retirement.RetirementNumber}",
                PreviousStatus = nameof(AdministrativeStatus.DISPONIBLE),
                NewStatus = nameof(AdministrativeStatus.DADO_DE_BAJA),
                ReferenceType = "RETIREMENT",
                ReferenceId = retirement.Id,
                PerformedById = currentUserId,
                PerformedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
}
