namespace ScoutAsset.Server.Application.Models.Dirigentes;

public record CreateDirigenteRequest(
    string CI, 
    string Nombres, 
    string Apellidos, 
    string Correo, 
    string Telefono, 
    bool HabilitadoParaCustodio
);
