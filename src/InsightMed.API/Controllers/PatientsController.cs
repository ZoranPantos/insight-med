using AutoMapper;
using InsightMed.API.Models;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.Patients.Commands;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class PatientsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public PatientsController(ISender sender, IMapper mapper)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<GetAllPatientsQueryResponse>> GetAllAsync(
        [FromQuery] string? searchKey,
        [FromQuery] int? pageNumber)
    {
        var response = await _sender.Send(new GetAllPatientsQuery(searchKey, pageNumber));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetPatientByIdQueryResponse>> GetByIdAsync(
        int id,
        [FromQuery] int pageNumber)
    {
        var response = await _sender.Send(new GetPatientByIdQuery(id, pageNumber));
        return Ok(response);
    }

    [HttpGet("{id:int}/labReports")]
    public async Task<ActionResult<GetAllLabReportsQueryResponse>> GetLabReportsByPatientIdAsync(int id)
    {
        var response = await _sender.Send(new GetAllLabReportsByPatientIdQuery(id));
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> AddPatientAsync(AddPatientInputModel input)
    {
        var commandInputModel = _mapper.Map<AddPatientCommandInput>(input);
        await _sender.Send(new AddPatientCommand(commandInputModel));

        return Ok();
    }

    [HttpGet("{id:int}/evaluatedParameters")]
    public async Task<ActionResult> GetEvaluatedParametersByPatientIdAsync(int id)
    {
        var response = await _sender.Send(new GetEvaluatedParametersByPatientIdQuery(id));
        return Ok(response);
    }
}