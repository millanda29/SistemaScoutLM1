namespace ScoutAsset.Server.Application.Models.Users;

public record CreateUserRequest(
    string UserName, 
    string Email, 
    string? Password, 
    string? Role, 
    string CI, 
    string Nombres, 
    string Apellidos, 
    string Telefono
);
