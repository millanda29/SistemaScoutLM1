using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Location> Locations { get; }
    DbSet<Resource> Resources { get; }
    DbSet<ResourceAssignment> ResourceAssignments { get; }
    DbSet<Loan> Loans { get; }
    DbSet<LoanItem> LoanItems { get; }
    DbSet<Maintenance> Maintenances { get; }
    DbSet<PhysicalInventory> PhysicalInventories { get; }
    DbSet<PhysicalInventoryLocation> PhysicalInventoryLocations { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryReconciliation> InventoryReconciliations { get; }
    DbSet<Loss> Losses { get; }
    DbSet<Retirement> Retirements { get; }
    DbSet<Movement> Movements { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Document> Documents { get; }
    DbSet<Dirigente> Dirigentes { get; }
    DbSet<UserProfile> UserProfiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
