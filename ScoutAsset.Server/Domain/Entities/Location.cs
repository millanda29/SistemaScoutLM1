using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
