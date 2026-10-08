using Microsoft.AspNetCore.Identity;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Models.Users;

namespace ScoutAsset.Server.Application.Services;

public record UserDetailDto(
    string Id, 
    string UserName, 
    string Email, 
    string CI, 
    string Nombres, 
    string Apellidos, 
    string Telefono, 
    List<string> Roles
);

public interface IUserService
{
    Task<List<UserDetailDto>> GetAllAsync();
    Task<UserDetailDto?> GetByIdAsync(string id);
    Task<IdentityUser> CreateAsync(CreateUserRequest request);
    Task UpdateAsync(string id, UpdateUserRequest request);
    Task DeleteAsync(string id);
    Task<ResourceImportResult> ImportUsersAsync(Stream stream, string fileName);
}
