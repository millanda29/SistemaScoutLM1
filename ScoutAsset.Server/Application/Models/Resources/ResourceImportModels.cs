namespace ScoutAsset.Server.Application.Models.Resources;

public class ResourceImportRow
{
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public string? AcquisitionType { get; set; }
    public string? PhysicalCondition { get; set; }
    public string? AdministrativeStatus { get; set; }
}

public class ResourceImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> SuccessMessages { get; set; } = new();
}
