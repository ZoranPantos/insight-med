using AutoMapper;
using InsightMed.Application.LabParameters.Models;
using InsightMed.Application.LabParameters.Services.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace InsightMed.Application.LabParameters.Queries;

public sealed record GetAllLabParametersQuery : IRequest<GetAllLabParametersQueryResponse>;

public sealed class GetAllLabParametersQueryHandler
    : IRequestHandler<GetAllLabParametersQuery, GetAllLabParametersQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabParametersService _labParametersService;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = nameof(GetAllLabParametersQuery);

    public GetAllLabParametersQueryHandler(
        IMapper mapper,
        ILabParametersService labParametersService,
        IMemoryCache memoryCache)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task<GetAllLabParametersQueryResponse> Handle(
        GetAllLabParametersQuery request,
        CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<LabParameterResponse>? labParameterResponseCachedList))
        {
            return new GetAllLabParametersQueryResponse
            {
                LabParameters = labParameterResponseCachedList!,
                CachedResponse = true
            };
        }

        var labParameters = await _labParametersService.GetAllAsync();
        var labParameterResponseList = _mapper.Map<List<LabParameterResponse>>(labParameters);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60 * 24),
            Priority = CacheItemPriority.High
        };

        _memoryCache.Set(CacheKey, labParameterResponseList, cacheOptions);

        return new GetAllLabParametersQueryResponse
        {
            LabParameters = labParameterResponseList
        };
    }
}