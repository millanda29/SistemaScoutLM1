using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Loans;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class LoanService : ILoanService
{
    private readonly IApplicationDbContext _context;

    public LoanService(IApplicationDbContext context) => _context = context;

    public async Task<List<Loan>> GetAllAsync(string? status)
    {
        var query = _context.Loans
            .Include(l => l.Items)
            .ThenInclude(li => li.Resource)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);

        return await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id) =>
        await _context.Loans
            .Include(l => l.Items)
            .ThenInclude(li => li.Resource)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loan> CreateRequestAsync(RequestLoanRequest request, string currentUserId)
    {
        var date = DateTime.UtcNow;
        var requestNumber = $"PREST-{date:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";

        var loan = new Loan
        {
            RequestNumber = requestNumber,
            RequesterId = currentUserId,
            Status = nameof(LoanStatus.SOLICITADO),
            Reason = request.Reason,
            ExpectedExitDate = request.ExpectedExitDate,
            ExpectedReturnDate = request.ExpectedReturnDate,
            CreatedById = currentUserId,
            RequesterType = request.RequesterType ?? "INTERNO",
            LoanDate = request.LoanDate,
            PdfDocumentPath = request.PdfDocumentPath
        };

        foreach (var resourceId in request.ResourceIds)
        {
            var resource = await _context.Resources.FindAsync(resourceId)
                ?? throw new InvalidOperationException($"Recurso {resourceId} no encontrado");

            if (resource.AdministrativeStatus != nameof(AdministrativeStatus.DISPONIBLE) &&
                resource.AdministrativeStatus != nameof(AdministrativeStatus.ASIGNADO))
                throw new InvalidOperationException($"El recurso '{resource.Code} - {resource.Name}' no está disponible para préstamo (Estado: {resource.AdministrativeStatus}).");

            if (resource.PhysicalCondition == nameof(PhysicalCondition.DANIADO) ||
                resource.PhysicalCondition == "DANNADO" ||
                resource.PhysicalCondition == "DAÑADO" ||
                resource.PhysicalCondition == "EN_REPARACION")
                throw new InvalidOperationException($"El recurso '{resource.Code} - {resource.Name}' no se puede prestar porque se encuentra en condición '{resource.PhysicalCondition}'.");

            loan.Items.Add(new LoanItem
            {
                ResourceId = resourceId,
                ConditionAtDelivery = resource.PhysicalCondition
            });
        }

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task ApproveAsync(int id, ApproveLoanRequest request, string currentUserId)
    {
        var loan = await _context.Loans
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new KeyNotFoundException("Préstamo no encontrado");

        if (loan.Status != nameof(LoanStatus.SOLICITADO))
            throw new InvalidOperationException("El préstamo no está en estado SOLICITADO");

        loan.Status = nameof(LoanStatus.APROBADO);
        loan.ApproverId = currentUserId;
        loan.ApprovalType = request.ApprovalType;
        loan.ApprovalDate = DateTime.UtcNow;
        loan.ApprovalObservations = request.Observations;
        loan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, RejectLoanRequest request, string currentUserId)
    {
        var loan = await _context.Loans.FindAsync(id)
            ?? throw new KeyNotFoundException("Préstamo no encontrado");

        loan.Status = nameof(LoanStatus.RECHAZADO);
        loan.ApproverId = currentUserId;
        loan.RejectionReason = request.Reason;
        loan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeliverAsync(int id, string currentUserId)
    {
        var loan = await _context.Loans
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new KeyNotFoundException("Préstamo no encontrado");

        if (!loan.CanDeliver())
            throw new InvalidOperationException("El préstamo debe estar APROBADO para entregar");

        loan.Status = nameof(LoanStatus.ENTREGADO);
        loan.DeliveredById = currentUserId;
        loan.ActualDeliveryDate = DateTime.UtcNow;
        loan.UpdatedAt = DateTime.UtcNow;

        foreach (var item in loan.Items)
        {
            var resource = await _context.Resources.FindAsync(item.ResourceId);
            if (resource != null)
            {
                resource.AdministrativeStatus = nameof(AdministrativeStatus.PRESTADO);
                resource.UpdatedAt = DateTime.UtcNow;

                _context.Movements.Add(new Movement
                {
                    ResourceId = resource.Id,
                    Type = nameof(MovementType.PRESTAMO),
                    Description = $"Préstamo #{loan.RequestNumber}",
                    PreviousStatus = nameof(AdministrativeStatus.DISPONIBLE),
                    NewStatus = nameof(AdministrativeStatus.PRESTADO),
                    ReferenceType = "LOAN",
                    ReferenceId = loan.Id,
                    PerformedById = currentUserId,
                    PerformedAt = DateTime.UtcNow
                });
            }
        }

        loan.Status = nameof(LoanStatus.PRESTADO);
        await _context.SaveChangesAsync();
    }

    public async Task ReturnLoanAsync(int id, ReturnLoanRequest request, string currentUserId)
    {
        var loan = await _context.Loans
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new KeyNotFoundException("Préstamo no encontrado");

        if (loan.Status != nameof(LoanStatus.PRESTADO))
            throw new InvalidOperationException("El préstamo debe estar PRESTADO para devolver");

        loan.Status = nameof(LoanStatus.DEVUELTO);
        loan.ReceivedById = currentUserId;
        loan.ActualReturnDate = DateTime.UtcNow;
        loan.UpdatedAt = DateTime.UtcNow;

        foreach (var item in loan.Items)
        {
            item.ConditionAtReturn = request.ConditionAtReturn;
            item.ReturnObservations = request.Observations;
            item.DamagesDetected = request.DamagesDetected;

            var resource = await _context.Resources.FindAsync(item.ResourceId);
            if (resource != null)
            {
                resource.PhysicalCondition = request.ConditionAtReturn;
                resource.AdministrativeStatus = request.ConditionAtReturn == nameof(PhysicalCondition.DANIADO)
                    ? nameof(AdministrativeStatus.EN_MANTENIMIENTO)
                    : nameof(AdministrativeStatus.DISPONIBLE);
                resource.UpdatedAt = DateTime.UtcNow;

                _context.Movements.Add(new Movement
                {
                    ResourceId = resource.Id,
                    Type = nameof(MovementType.DEVOLUCION),
                    Description = $"Devolución préstamo #{loan.RequestNumber}",
                    PreviousStatus = nameof(AdministrativeStatus.PRESTADO),
                    NewStatus = resource.AdministrativeStatus,
                    ReferenceType = "LOAN",
                    ReferenceId = loan.Id,
                    PerformedById = currentUserId,
                    PerformedAt = DateTime.UtcNow
                });
            }
        }

        loan.Status = nameof(LoanStatus.FINALIZADO);
        await _context.SaveChangesAsync();
    }
}
