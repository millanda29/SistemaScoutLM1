using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Services;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.GetAsync();
        return Ok(data);
    }
}
