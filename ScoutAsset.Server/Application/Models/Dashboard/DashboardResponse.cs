namespace ScoutAsset.Server.Application.Models.Dashboard;

public record CategoryStatDto(string Category, int Count, int Disponibles);
public record RecentMovementDto(string ResourceCode, string ResourceName, string Type, string Description, DateTime PerformedAt);

public record DashboardResponse(
    int Total,
    int Disponibles,
    int Prestados,
    int EnMantenimiento,
    int NoLocalizados,
    int Perdidos,
    int DadosDeBaja,
    int Daniados,
    int PrestamosVencidos,
    decimal TotalAcquisitionCost,
    decimal TotalAppraisedValue,
    List<CategoryStatDto> CategoryStats,
    List<RecentMovementDto> RecentMovements);
