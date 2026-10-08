using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Domain.Entities;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IApplicationDbContext _context;

    public ProfileController(UserManager<IdentityUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            Roles = roles,
            CI = profile?.CI ?? string.Empty,
            Nombres = profile?.Nombres ?? string.Empty,
            Apellidos = profile?.Apellidos ?? string.Empty,
            Telefono = profile?.Telefono ?? string.Empty
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            profile = new UserProfile { UserId = userId };
            _context.UserProfiles.Add(profile);
        }

        profile.CI = request.CI;
        profile.Nombres = request.Nombres;
        profile.Apellidos = request.Apellidos;
        profile.Telefono = request.Telefono;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Perfil actualizado correctamente" });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        return Ok(new { message = "Contraseña cambiada exitosamente" });
    }
}

public record UpdateProfileRequest(string CI, string Nombres, string Apellidos, string Telefono);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
