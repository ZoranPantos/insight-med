using InsightMed.API.DTOs;
using InsightMed.Application.LabRequests.Commands;
using InsightMed.Application.LabRequests.Models;
using InsightMed.Application.LabRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class LabRequestsController : ControllerBase
{
    private readonly ISender _sender;

    public LabRequestsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllLabRequestsQueryResponse>> GetAllAsync()
    {
        var response = await _sender.Send(new GetAllLabRequestsQuery());
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(LabRequestInputModel input)
    {
        var command = new CreateLabRequestCommand(input.PatientId, input.LabParameterIds);
        await _sender.Send(command);
        return NoContent();
    }
}
