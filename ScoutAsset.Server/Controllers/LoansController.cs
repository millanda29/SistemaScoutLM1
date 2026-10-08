using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutAsset.Server.Application.Models.Loans;
using ScoutAsset.Server.Application.Services;
using System.Security.Claims;

namespace ScoutAsset.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _service;

    public LoansController(ILoanService service) => _service = service;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var loans = await _service.GetAllAsync(status);
        var result = loans.Select(l => new
        {
            l.Id, l.RequestNumber, l.RequesterId, l.ApproverId, l.Status,
            l.Reason, l.ApprovalType, l.ExpectedExitDate, l.ExpectedReturnDate,
            l.ActualDeliveryDate, l.ActualReturnDate, l.CreatedAt,
            Items = l.Items.Select(i => new
            {
                i.Id, i.ResourceId, i.ConditionAtDelivery, i.ConditionAtReturn,
                ResourceCode = i.Resource?.Code, ResourceName = i.Resource?.Name
            })
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _service.GetByIdAsync(id);
        if (loan == null) return NotFound();
        return Ok(new
        {
            loan.Id, loan.RequestNumber, loan.RequesterId, loan.ApproverId,
            loan.Status, loan.Reason, loan.ApprovalType, loan.ExpectedExitDate,
            loan.ExpectedReturnDate, loan.ActualDeliveryDate, loan.ActualReturnDate,
            loan.CreatedAt,
            Items = loan.Items.Select(i => new
            {
                i.Id, i.ResourceId, i.ConditionAtDelivery, i.ConditionAtReturn,
                ResourceCode = i.Resource?.Code, ResourceName = i.Resource?.Name
            })
        });
    }

    [Authorize(Roles = "ADMIN,SUPERINTENDENTE,AUXILIAR")]
    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] RequestLoanRequest request)
    {
        try
        {
            var loan = await _service.CreateRequestAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, new
            {
                loan.Id, loan.RequestNumber, loan.RequesterId, loan.Status,
                loan.Reason, loan.ExpectedExitDate, loan.ExpectedReturnDate,
                loan.CreatedAt
            });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveLoanRequest request)
    {
        try
        {
            await _service.ApproveAsync(id, request, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectLoanRequest request)
    {
        try
        {
            await _service.RejectAsync(id, request, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id}/deliver")]
    public async Task<IActionResult> Deliver(int id)
    {
        try
        {
            await _service.DeliverAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/return")]
    public async Task<IActionResult> ReturnLoan(int id, [FromBody] ReturnLoanRequest request)
    {
        try
        {
            await _service.ReturnLoanAsync(id, request, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN,SUPERINTENDENTE,AUXILIAR")]
    public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Archivo no proporcionado o vacío" });

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".pdf")
            return BadRequest(new { message = "Solo se permiten documentos PDF" });

        // Ensure the directory exists inside wwwroot
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "loans");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/loans/{fileName}";
        return Ok(new { filePath = relativePath });
    }
}
