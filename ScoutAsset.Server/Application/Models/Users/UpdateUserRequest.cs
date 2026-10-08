namespace ScoutAsset.Server.Application.Models.Users;

public record UpdateUserRequest(string? UserName, string? Email, List<string>? Roles);
