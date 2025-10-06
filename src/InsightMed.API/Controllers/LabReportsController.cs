using InsightMed.Application.LabReports.Models;
using InsightMed.Application.LabReports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class LabReportsController : ControllerBase
{
    private readonly ISender _sender;

    public LabReportsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllLabReportsQueryResponse>> GetAllAsync()
    {
        var response = await _sender.Send(new GetAllLabReportsQuery());
        return Ok(response);
    }

    [HttpGet("{patientId:int}")]
    public async Task<ActionResult<GetAllLabReportsQueryResponse>> GetAllByPatientIdAsync(int patientId)
    {
        var response = await _sender.Send(new GetAllLabReportsByPatientIdQuery(patientId));
        return Ok(response);
    }
}
