using Microsoft.AspNetCore.Identity;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Services;

namespace ScoutAsset.Server.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }

    public async Task<object> LoginAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var roles = await _userManager.GetRolesAsync(user);
        
        var allowedRoles = new[] { "ADMIN", "SUPERINTENDENTE", "AUXILIAR", "JEFE_GRUPO" };
        var hasAccess = roles.Any(r => allowedRoles.Contains(r));
        if (!hasAccess)
            throw new UnauthorizedAccessException("Acceso denegado: rol no autorizado para acceder al sistema");

        var token = _jwtService.GenerateToken(user.Id, user.UserName!, roles);

        return new { token, userName = user.UserName, email = user.Email, roles };
    }

    public async Task RegisterAsync(string userName, string email, string password, string role)
    {
        var user = new IdentityUser { UserName = userName, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);
    }
}
