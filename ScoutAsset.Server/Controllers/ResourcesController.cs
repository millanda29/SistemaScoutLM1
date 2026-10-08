using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _service;

    public ResourcesController(IResourceService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? locationId,
        [FromQuery] string? status)
    {
        var resources = await _service.GetAllAsync(search, categoryId, locationId, status);
        var result = resources.Select(r => new
        {
            r.Id, r.Code, r.Name, r.Description, r.CategoryId,
            Category = r.Category?.Name, r.Brand, r.Model, r.SerialNumber,
            r.PhysicalCondition, r.AdministrativeStatus, r.LocationId,
            Location = r.Location?.Name, r.CurrentResponsibleId,
            r.AcquisitionType, r.AcquisitionCost, r.AppraisedValue, r.CreatedAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var resource = await _service.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return Ok(new
        {
            resource.Id, resource.Code, resource.Name, resource.Description,
            resource.CategoryId, Category = resource.Category?.Name,
            resource.Brand, resource.Model, resource.SerialNumber,
            resource.PhysicalCondition, resource.AdministrativeStatus,
            resource.LocationId, Location = resource.Location?.Name,
            resource.CurrentResponsibleId, resource.AcquisitionType,
            resource.AcquisitionCost, resource.AppraisedValue, resource.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResourceRequest request)
    {
        try
        {
            var resource = await _service.CreateAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = resource.Id }, new
            {
                resource.Id, resource.Code, resource.Name, resource.Description,
                resource.CategoryId, Category = resource.Category?.Name,
                resource.Brand, resource.Model, resource.SerialNumber,
                resource.PhysicalCondition, resource.AdministrativeStatus,
                resource.LocationId, Location = resource.Location?.Name,
                resource.CurrentResponsibleId, resource.AcquisitionType,
                resource.AcquisitionCost, resource.AppraisedValue, resource.CreatedAt
            });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateResourceRequest request)
    {
        try
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/location")]
    public async Task<IActionResult> ChangeLocation(int id, [FromBody] ChangeLocationRequest request)
    {
        try
        {
            await _service.ChangeLocationAsync(id, request, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/responsible")]
    public async Task<IActionResult> ChangeResponsible(int id, [FromBody] ChangeResponsibleRequest request)
    {
        try
        {
            await _service.ChangeResponsibleAsync(id, request, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("{id}/movements")]
    public async Task<IActionResult> GetMovements(int id)
    {
        var movements = await _service.GetMovementsAsync(id);
        return Ok(movements);
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
    public async Task<IActionResult> Import([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Por favor selecciona un archivo Excel (.xlsx, .xls o .csv) válido." });

        using var stream = file.OpenReadStream();
        var result = await _service.ImportResourcesAsync(stream, file.FileName, CurrentUserId);
        return Ok(result);
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        var csvHeader = "Código,Nombre,Descripción,Categoría,Ubicación,Marca,Modelo,Serie,Costo,Adquisición,Condición\n";
        var csvSample1 = "MOB-001,Mesa Plegable 1.80m,Mesa de plástico reforzada para campamento,Mobiliario,Bodega Principal,Lifetime,PT-180,SN-99812,45.50,COMPRA,BUENO\n";
        var csvSample2 = "CAR-002,Carpa de 4 personas,Carpa impermeable de lona,Carpas,Cuartel Scout,Coleman,Sundome 4,SN-44123,120.00,DONACION,BUENO\n";

        var csvContent = string.Concat(csvHeader, csvSample1, csvSample2);
        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csvContent)).ToArray();

        return File(bytes, "text/csv", "Plantilla_Importacion_Recursos.csv");
    }
}
