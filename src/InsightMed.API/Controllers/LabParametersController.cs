using InsightMed.Application.LabParameters.Models;
using InsightMed.Application.LabParameters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class LabParametersController : ControllerBase
{
    private readonly ISender _sender;

    public LabParametersController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllLabParametersQueryResponse>> GetAllAsync()
    {
        var response = await _sender.Send(new GetAllLabParametersQuery());
        return Ok(response);
    }
}
