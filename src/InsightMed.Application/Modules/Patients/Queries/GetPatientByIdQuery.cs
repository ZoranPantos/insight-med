using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Application.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetPatientByIdQuery(int Id, int PageNumber) : IRequest<GetPatientByIdQueryResponse>;

public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, GetPatientByIdQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;
    private readonly PagingOptions _pagingOptions;

    public GetPatientByIdQueryHandler(
        IMapper mapper,
        IPatientsService patientsService,
        IOptions<PagingOptions> pagingOptions)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
        _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions));
    }

    public async Task<GetPatientByIdQueryResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = _pagingOptions.PatientDetailsLabRequestsPageSize;

        var (Patient, PagedLabRequests, TotalCount) = await _patientsService.GetByIdWithLabRequestsPagedAsync(request.Id, pageNumber, pageSize);

        if (Patient is null)
            throw new ResourceNotFoundException($"Patient with ID {request.Id} not found");

        Patient.LabRequests = PagedLabRequests;

        var response = _mapper.Map<GetPatientByIdQueryResponse>(Patient);

        response.PageNumber = pageNumber;
        response.PageSize = pageSize;
        response.TotalCount = TotalCount;

        return response;
    }
}