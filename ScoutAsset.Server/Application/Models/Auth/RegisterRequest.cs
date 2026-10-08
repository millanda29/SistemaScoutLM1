namespace ScoutAsset.Server.Application.Models.Auth;

public record RegisterRequest(string UserName, string Email, string Password, string Role = "SOLICITANTE");
