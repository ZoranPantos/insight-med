using InsightMed.API.DTOs;
using InsightMed.Application.Modules.LabRequests.Commands;
using InsightMed.Application.Modules.LabRequests.Models;
using InsightMed.Application.Modules.LabRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class LabRequestsController : ControllerBase
{
    private readonly ISender _sender;

    public LabRequestsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllLabRequestsQueryResponse>> GetAllAsync([FromQuery] string? searchKey)
    {
        var response = await _sender.Send(new GetAllLabRequestsQuery(searchKey));
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