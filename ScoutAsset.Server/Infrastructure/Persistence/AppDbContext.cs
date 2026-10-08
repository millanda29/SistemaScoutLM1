using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceAssignment> ResourceAssignments => Set<ResourceAssignment>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanItem> LoanItems => Set<LoanItem>();
    public DbSet<Maintenance> Maintenances => Set<Maintenance>();
    public DbSet<PhysicalInventory> PhysicalInventories => Set<PhysicalInventory>();
    public DbSet<PhysicalInventoryLocation> PhysicalInventoryLocations => Set<PhysicalInventoryLocation>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryReconciliation> InventoryReconciliations => Set<InventoryReconciliation>();
    public DbSet<Loss> Losses => Set<Loss>();
    public DbSet<Retirement> Retirements => Set<Retirement>();
    public DbSet<Movement> Movements => Set<Movement>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Dirigente> Dirigentes => Set<Dirigente>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
