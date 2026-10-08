using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Resource : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string AcquisitionType { get; set; } = nameof(Enums.AcquisitionType.OTRO);
    public decimal? AcquisitionCost { get; set; }
    public decimal? AppraisedValue { get; set; }
    public string PhysicalCondition { get; set; } = nameof(Enums.PhysicalCondition.BUENO);
    public string AdministrativeStatus { get; set; } = nameof(Enums.AdministrativeStatus.REGISTRADO);
    public int LocationId { get; set; }
    public string? CurrentResponsibleId { get; set; }
    public int? MaintenanceFrequencyDays { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? Observations { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedById { get; set; } = string.Empty;

    public Category Category { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public ICollection<ResourceAssignment> Assignments { get; set; } = new List<ResourceAssignment>();
    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
    public Loss? Loss { get; set; }
    public Retirement? Retirement { get; set; }

    public bool CanBeAvailable()
    {
        return PhysicalCondition != nameof(Enums.PhysicalCondition.DANIADO)
            && AdministrativeStatus != nameof(Enums.AdministrativeStatus.PRESTADO)
            && AdministrativeStatus != nameof(Enums.AdministrativeStatus.EN_MANTENIMIENTO)
            && AdministrativeStatus != nameof(Enums.AdministrativeStatus.NO_LOCALIZADO)
            && AdministrativeStatus != nameof(Enums.AdministrativeStatus.PERDIDO)
            && AdministrativeStatus != nameof(Enums.AdministrativeStatus.DADO_DE_BAJA);
    }

    public bool CanChangeToAvailable()
    {
        return AdministrativeStatus != nameof(Enums.AdministrativeStatus.DADO_DE_BAJA);
    }
}
