using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Dirigentes;
using ScoutAsset.Server.Application.Services;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DirigentesController : ControllerBase
{
    private readonly IDirigenteService _service;

    public DirigentesController(IDirigenteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dirigentes = await _service.GetAllAsync();
        return Ok(dirigentes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dirigente = await _service.GetByIdAsync(id);
        if (dirigente == null) return NotFound();
        return Ok(dirigente);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDirigenteRequest request)
    {
        try
        {
            var dirigente = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = dirigente.Id }, dirigente);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDirigenteRequest request)
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
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Por favor selecciona un archivo válido." });

        using var stream = file.OpenReadStream();
        var result = await _service.ImportDirigentesAsync(stream, file.FileName);
        return Ok(result);
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        var csvHeader = "Cédula,Nombres,Apellidos,Correo,Teléfono,Custodio\n" +
                        "1723456789,Carlos Eduardo,Mendoza Silva,carlos.mendoza@scouts.org.ec,0991234567,SI\n" +
                        "1500987654,Ana Patricia,Ríos Salazar,ana.rios@scouts.org.ec,0987654321,SI\n";

        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csvHeader)).ToArray();
        return File(bytes, "text/csv", "Plantilla_Importacion_Dirigentes.csv");
    }
}
