using ScoutAsset.Server.Domain.Common;

namespace ScoutAsset.Server.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public int NextNumber { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
