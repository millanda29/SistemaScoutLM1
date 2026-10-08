using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string CI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
