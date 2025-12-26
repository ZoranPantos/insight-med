using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Models;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Options;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace InsightMed.Application.Modules.LabParameters.Queries;

public sealed record GetAllLabParametersQuery : IRequest<GetAllLabParametersQueryResponse>;

public sealed class GetAllLabParametersQueryHandler
    : IRequestHandler<GetAllLabParametersQuery, GetAllLabParametersQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabParametersService _labParametersService;
    private readonly IMemoryCache _memoryCache;
    private readonly Options.MemoryCacheOptions _memoryCacheOptions;
    private const string CacheKey = nameof(GetAllLabParametersQuery);

    public GetAllLabParametersQueryHandler(
        IMapper mapper,
        ILabParametersService labParametersService,
        IMemoryCache memoryCache,
        IOptions<Options.MemoryCacheOptions> memoryCacheOptions)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _memoryCacheOptions = memoryCacheOptions.Value ?? throw new ArgumentNullException(nameof(memoryCacheOptions));
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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_memoryCacheOptions.AbsoluteExpirationMinutes),
            Priority = CacheItemPriority.High
        };

        _memoryCache.Set(CacheKey, labParameterResponseList, cacheOptions);

        return new GetAllLabParametersQueryResponse
        {
            LabParameters = labParameterResponseList
        };
    }
}