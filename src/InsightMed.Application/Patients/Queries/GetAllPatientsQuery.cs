using AutoMapper;
using InsightMed.Application.Patients.Models;
using InsightMed.Application.Patients.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Patients.Queries;

public record GetAllPatientsQuery : IRequest<GetAllPatientsQueryResponse>;

public class GetAllPatientsQueryHanlder : IRequestHandler<GetAllPatientsQuery, GetAllPatientsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;

    public GetAllPatientsQueryHanlder(IPatientsService patientsService, IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
    }

    public async Task<GetAllPatientsQueryResponse> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientsService.GetAllPatients();

        var response = new GetAllPatientsQueryResponse
        {
            Patients = _mapper.Map<List<PatientLiteResponse>>(patients)
        };

        return response;
    }
}