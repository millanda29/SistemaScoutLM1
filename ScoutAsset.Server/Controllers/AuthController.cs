using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Auth;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;
using System.Web;

namespace ScoutAsset.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        IAuthService service, 
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender)
    {
        _service = service;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _service.LoginAsync(request.UserName, request.Password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _service.RegisterAsync(request.UserName, request.Email, request.Password, request.Role);
            return Ok(new { message = "Usuario creado exitosamente" });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Ok(new { message = "Si el correo está registrado, recibirás un enlace de restablecimiento" });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var encodedEmail = HttpUtility.UrlEncode(user.Email);
        var resetLink = $"https://localhost:49698/reset-password?token={encodedToken}&email={encodedEmail}";

        var subject = "Restablece tu contraseña - ScoutAsset";
        var body = ScoutAsset.Server.Infrastructure.Services.EmailTemplates.GetForgotPasswordEmail(user.UserName ?? string.Empty, resetLink);

        await _emailSender.SendEmailAsync(user.Email!, subject, body);

        return Ok(new { message = "Si el correo está registrado, recibirás un enlace de restablecimiento" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "El correo electrónico no es válido" });
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        return Ok(new { message = "Contraseña restablecida exitosamente" });
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var user = !string.IsNullOrEmpty(request.Email) 
            ? await _userManager.FindByEmailAsync(request.Email)
            : await _userManager.FindByNameAsync(request.UserName);

        if (user == null)
        {
            return BadRequest(new { message = "Usuario no encontrado" });
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }
}

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record SetPasswordRequest(string Email, string UserName, string CurrentPassword, string NewPassword);
