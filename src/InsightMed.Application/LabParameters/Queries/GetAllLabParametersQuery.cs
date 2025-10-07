using AutoMapper;
using InsightMed.Application.LabParameters.Models;
using InsightMed.Application.LabParameters.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.LabParameters.Queries;

public sealed record GetAllLabParametersQuery : IRequest<GetAllLabParametersQueryResponse>;

public sealed class GetAllLabParametersQueryHandler
    : IRequestHandler<GetAllLabParametersQuery, GetAllLabParametersQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabParametersService _labParametersService;

    public GetAllLabParametersQueryHandler(IMapper mapper, ILabParametersService labParametersService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
    }

    public async Task<GetAllLabParametersQueryResponse> Handle(
        GetAllLabParametersQuery request,
        CancellationToken cancellationToken)
    {
        var labParameters = await _labParametersService.GetAllAsync();

        var response = new GetAllLabParametersQueryResponse
        {
            LabParameters = _mapper.Map<List<LabParameterResponse>>(labParameters)
        };

        return response;
    }
}