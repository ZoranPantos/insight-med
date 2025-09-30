using AutoMapper;
using InsightMed.Application.Exceptions;
using InsightMed.Application.Patients.Models;
using InsightMed.Application.Patients.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Patients.Queries;

public record GetPatientByIdQuery(long Id) : IRequest<GetPatientByIdQueryResponse>;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, GetPatientByIdQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;

    public GetPatientByIdQueryHandler(IMapper mapper, IPatientsService patientsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentException(nameof(patientsService));
    }

    public async Task<GetPatientByIdQueryResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientsService.GetPatientByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Patient with ID {request.Id} not found");

        var response = _mapper.Map<GetPatientByIdQueryResponse>(patient);

        return response;
    }
}