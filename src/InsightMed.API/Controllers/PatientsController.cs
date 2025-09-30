using InsightMed.Application.Patients.Models;
using InsightMed.Application.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
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
}
