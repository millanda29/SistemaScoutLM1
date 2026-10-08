namespace ScoutAsset.Server.Application.Services;

public interface IAuthService
{
    Task<object> LoginAsync(string userName, string password);
    Task RegisterAsync(string userName, string email, string password, string role);
}
