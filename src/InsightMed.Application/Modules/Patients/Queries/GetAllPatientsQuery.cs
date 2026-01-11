using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using MediatR;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetAllPatientsQuery(string? SearchKey) : IRequest<GetAllPatientsQueryResponse>;

public sealed class GetAllPatientsQueryHanlder : IRequestHandler<GetAllPatientsQuery, GetAllPatientsQueryResponse>
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
        List<Patient> patients = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
            patients = await _patientsService.GetAllAsync();
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            patients = await _patientsService.SearchByTokensAsync(tokens);
        }

        var response = new GetAllPatientsQueryResponse
        {
            Patients = _mapper.Map<List<PatientLiteResponse>>(patients)
        };

        return response;
    }
}