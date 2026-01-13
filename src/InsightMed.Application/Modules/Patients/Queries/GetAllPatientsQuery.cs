using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetAllPatientsQuery(string? SearchKey, int? PageNumber) : IRequest<GetAllPatientsQueryResponse>;

public sealed class GetAllPatientsQueryHanlder : IRequestHandler<GetAllPatientsQuery, GetAllPatientsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;
    private readonly PagingOptions _pagingOptions;

    public GetAllPatientsQueryHanlder(
        IPatientsService patientsService,
        IMapper mapper,
        IOptions<PagingOptions> pagingOptions)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
        _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions));
    }

    public async Task<GetAllPatientsQueryResponse> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        bool usePagingNoSearch = request.PageNumber.HasValue && request.PageNumber > 0;

        int pageNumber = request.PageNumber.HasValue && request.PageNumber > 0 ? request.PageNumber.Value : 1;
        int pageSize = _pagingOptions.PatientsPageSize;
        int totalCount;
        List<Patient> patients = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
        {
            if (usePagingNoSearch)
            {
                var (Items, TotalCount) = await _patientsService.GetAllPagedAsync(pageNumber, pageSize);
                patients = Items;
                totalCount = TotalCount;
            }
            else
            {
                var (Items, TotalCount) = await _patientsService.GetAllAsync();
                patients = Items;
                totalCount = TotalCount;
                pageNumber = 1;
                pageSize = TotalCount;
            }
        }
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            var (Items, TotalCount) = await _patientsService.SearchByTokensPagedAsync(tokens, pageNumber, pageSize);
            patients = Items;
            totalCount = TotalCount;
        }

        var response = new GetAllPatientsQueryResponse
        {
            Patients = _mapper.Map<List<PatientLiteResponse>>(patients),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return response;
    }
}