using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetPatientByIdQuery(int Id) : IRequest<GetPatientByIdQueryResponse>;

public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, GetPatientByIdQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;

    public GetPatientByIdQueryHandler(IMapper mapper, IPatientsService patientsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
    }

    public async Task<GetPatientByIdQueryResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientsService.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Patient with ID {request.Id} not found");

        var response = _mapper.Map<GetPatientByIdQueryResponse>(patient);

        return response;
    }
}