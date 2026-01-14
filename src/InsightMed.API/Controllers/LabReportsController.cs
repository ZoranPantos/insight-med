using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class LabReportsController : ControllerBase
{
    private readonly ISender _sender;

    public LabReportsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllLabReportsQueryResponse>> GetAllAsync(
        [FromQuery] string? searchKey,
        [FromQuery] int pageNumber)
    {
        var response = await _sender.Send(new GetAllLabReportsQuery(searchKey, pageNumber));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetLabReportByIdQueryResponse>> GetByIdAsync(int id)
    {
        var response = await _sender.Send(new GetLabReportByIdQuery(id));
        return Ok(response);
    }

    [HttpGet("{id:int}/export")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var response = await _sender.Send(new ExportLabReportQuery(id));
        return File(response.Data, response.ContentType, response.FileName);
    }
}