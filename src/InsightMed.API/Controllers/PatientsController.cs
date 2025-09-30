using InsightMed.Application.Patients.Models;
using InsightMed.Application.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class PatientsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllPatientsQueryResponse>> PatientsAsync()
    {
        var response = await _sender.Send(new GetAllPatientsQuery());
        return Ok(response);
    }

    // TODO: add validation for Id
    [HttpGet("{id:long}")]
    public async Task<ActionResult<GetPatientByIdQueryResponse>> GetByIdAsync(long id)
    {
        var response = await _sender.Send(new GetPatientByIdQuery(id));
        return Ok(response);
    }
}
