using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.PhysicalInventory;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhysicalInventoryController : ControllerBase
{
    private readonly IPhysicalInventoryService _service;

    public PhysicalInventoryController(IPhysicalInventoryService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    private static object MapToDto(PhysicalInventory i) => new
    {
        i.Id,
        i.InventoryNumber,
        i.Status,
        i.Description,
        i.StartDate,
        i.EndDate,
        i.CreatedAt,
        Locations = i.Locations.Select(l => new
        {
            l.LocationId,
            LocationName = l.Location?.Name
        }),
        Items = i.Items.Select(item => new
        {
            item.Id,
            item.PhysicalInventoryId,
            item.ResourceId,
            item.LocationId,
            item.Result,
            item.PhysicalCondition,
            item.FoundCode,
            item.FoundName,
            item.Observations,
            item.VerifiedAt
        })
    };

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventories = await _service.GetAllAsync();
        return Ok(inventories.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var inventory = await _service.GetByIdAsync(id);
        if (inventory == null) return NotFound();
        return Ok(MapToDto(inventory));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryRequest request)
    {
        var inventory = await _service.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = inventory.Id }, MapToDto(inventory));
    }

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            await _service.StartAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("{id}/items")]
    public async Task<IActionResult> RegisterItem(int id, [FromBody] RegisterItemRequest request)
    {
        try
        {
            var item = await _service.RegisterItemAsync(id, request, CurrentUserId);
            return Ok(item);
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/finish")]
    public async Task<IActionResult> Finish(int id)
    {
        try
        {
            await _service.FinishAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("{id}/reconcile")]
    public async Task<IActionResult> Reconcile(int id)
    {
        try
        {
            var reconciliation = await _service.ReconcileAsync(id, CurrentUserId);
            return Ok(reconciliation);
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id}/reconciliation")]
    public async Task<IActionResult> GetReconciliation(int id)
    {
        var reconciliation = await _service.GetReconciliationAsync(id);
        if (reconciliation == null) return NotFound();
        return Ok(reconciliation);
    }
}
