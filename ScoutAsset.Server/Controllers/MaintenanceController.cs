using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Models.Maintenance;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _service;

    public MaintenanceController(IMaintenanceService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? resourceId)
    {
        var list = await _service.GetAllAsync(status, resourceId);
        var result = list.Select(m => new
        {
            m.Id, m.ResourceId, ResourceCode = m.Resource?.Code,
            ResourceName = m.Resource?.Name, m.Type, m.Status, m.Description,
            m.ScheduledDate, m.CompletedDate, m.Cost, m.Result, m.NextMaintenanceDate
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var maintenance = await _service.GetByIdAsync(id);
        if (maintenance == null) return NotFound();
        return Ok(maintenance);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequest request)
    {
        try
        {
            var maintenance = await _service.CreateAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = maintenance.Id }, maintenance);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            await _service.StartAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteMaintenanceRequest request)
    {
        try
        {
            await _service.CompleteAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var overdue = await _service.GetOverdueAsync();
        var result = overdue.Select(m => new
        {
            m.Id, ResourceCode = m.Resource?.Code, ResourceName = m.Resource?.Name,
            m.Type, m.Description, m.ScheduledDate,
            DaysOverdue = EF.Functions.DateDiffDay(m.ScheduledDate, DateTime.UtcNow)
        });
        return Ok(result);
    }

    [HttpPost("auto-schedule")]
    public async Task<IActionResult> AutoSchedule([FromQuery] int intervalMonths = 6)
    {
        var count = await _service.AutoScheduleAsync(intervalMonths, CurrentUserId);
        return Ok(new { scheduledCount = count, message = $"Se auto-programaron {count} mantenimientos preventivos." });
    }
}
