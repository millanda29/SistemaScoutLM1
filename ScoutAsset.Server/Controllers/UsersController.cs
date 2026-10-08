using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Users;
using ScoutAsset.Server.Application.Services;

namespace ScoutAsset.Server.Controllers;

[Authorize(Roles = "ADMIN,SUPERINTENDENTE")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _service.CreateAsync(request);
            return Ok(new { user.Id, user.UserName, user.Email });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Por favor selecciona un archivo válido." });

        using var stream = file.OpenReadStream();
        var result = await _service.ImportUsersAsync(stream, file.FileName);
        return Ok(result);
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        var csvHeader = "Cédula,Nombres,Apellidos,Usuario,Correo,Teléfono,Rol\n" +
                        "1723456789,Juan Carlos,Pérez Gómez,juan.perez,juan.perez@scouts.org.ec,0991234567,DIRIGENTE\n" +
                        "1500987654,María Fernanda,López Torres,maria.lopez,maria.lopez@scouts.org.ec,0987654321,DIRIGENTE\n";

        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csvHeader)).ToArray();
        return File(bytes, "text/csv", "Plantilla_Importacion_Dirigentes.csv");
    }
}
