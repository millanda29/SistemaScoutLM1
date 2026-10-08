using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Dirigente : BaseEntity
{
    public string CI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool HabilitadoParaCustodio { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
