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
}
