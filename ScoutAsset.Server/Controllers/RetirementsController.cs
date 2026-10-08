using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Retirements;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RetirementsController : ControllerBase
{
    private readonly IRetirementService _service;

    public RetirementsController(IRetirementService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var retirements = await _service.GetAllAsync(status);
        var result = retirements.Select(r => new
        {
            r.Id, r.RetirementNumber, r.Status,
            ResourceCode = r.Resource?.Code, ResourceName = r.Resource?.Name,
            r.Reason, r.RequestedById, r.AuthorizedById, r.AuthorizedAt, r.ExecutedAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var retirement = await _service.GetByIdAsync(id);
        if (retirement == null) return NotFound();
        return Ok(retirement);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] RequestRetirementRequest request)
    {
        try
        {
            var retirement = await _service.CreateRequestAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = retirement.Id }, retirement);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/review")]
    public async Task<IActionResult> Review(int id)
    {
        try
        {
            await _service.ReviewAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/authorize")]
    public async Task<IActionResult> Authorize(int id)
    {
        try
        {
            await _service.AuthorizeAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectRetirementRequest request)
    {
        try
        {
            await _service.RejectAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/execute")]
    public async Task<IActionResult> Execute(int id)
    {
        try
        {
            await _service.ExecuteAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
