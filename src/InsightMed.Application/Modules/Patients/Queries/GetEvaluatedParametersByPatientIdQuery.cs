using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.Patients.Models;
using MediatR;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetEvaluatedParametersByPatientIdQuery(int Id) : IRequest<GetEvaluatedParametersByPatientIdQueryResponse>;

public sealed class GetEvaluatedParametersByPatientIdQueryHandler
    : IRequestHandler<GetEvaluatedParametersByPatientIdQuery, GetEvaluatedParametersByPatientIdQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabParametersService _labParametersService;

    public GetEvaluatedParametersByPatientIdQueryHandler(IMapper mapper, ILabParametersService labParametersService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
    }

    public async Task<GetEvaluatedParametersByPatientIdQueryResponse> Handle(
        GetEvaluatedParametersByPatientIdQuery request,
        CancellationToken cancellationToken)
    {
        var evaluatedLabParameters = await _labParametersService.GetAllByPatientIdAsync(request.Id);
        var evaluatedLabParametersResponse = _mapper.Map<List<EvaluatedLabParameterResponse>>(evaluatedLabParameters);

        return new() { EvaluatedLabParameters = evaluatedLabParametersResponse };
    }
}