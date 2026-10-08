using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Losses;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LossesController : ControllerBase
{
    private readonly ILossService _service;

    public LossesController(ILossService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var losses = await _service.GetAllAsync(status);
        var result = losses.Select(l => new
        {
            l.Id, l.LossNumber, l.Status,
            ResourceCode = l.Resource?.Code, ResourceName = l.Resource?.Name,
            l.Circumstances, l.ReportedById, l.ConfirmedAt, l.RecoveredAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loss = await _service.GetByIdAsync(id);
        if (loss == null) return NotFound();
        return Ok(loss);
    }

    [HttpPost]
    public async Task<IActionResult> Report([FromBody] ReportLossRequest request)
    {
        try
        {
            var loss = await _service.ReportAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = loss.Id }, loss);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        try
        {
            await _service.ConfirmAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/recover")]
    public async Task<IActionResult> Recover(int id)
    {
        try
        {
            await _service.RecoverAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/investigate")]
    public async Task<IActionResult> Investigate(int id, [FromBody] InvestigateRequest request)
    {
        try
        {
            await _service.InvestigateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
