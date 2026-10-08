using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Categories;
using ScoutAsset.Server.Application.Services;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryRequest request)
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
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Por favor selecciona un archivo Excel (.xlsx, .xls o .csv) válido." });

        using var stream = file.OpenReadStream();
        var result = await _service.ImportCategoriesAsync(stream, file.FileName);
        return Ok(result);
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        var csvHeader = "Nombre,Prefijo,Descripción\n";
        var csvSample1 = "Mobiliario,MOB,Muebles mesas y sillas para actividades scout\n";
        var csvSample2 = "Carpas,CAR,Carpas de lona e impermeables\n";

        var csvContent = string.Concat(csvHeader, csvSample1, csvSample2);
        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csvContent)).ToArray();

        return File(bytes, "text/csv", "Plantilla_Importacion_Categorias.csv");
    }
}
