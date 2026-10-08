namespace ScoutAsset.Server.Application.Models.Categories;

public record CreateCategoryRequest(string Name, string? Description, string Prefix);
