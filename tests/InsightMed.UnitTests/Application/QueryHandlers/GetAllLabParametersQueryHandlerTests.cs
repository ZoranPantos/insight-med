using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Models;
using InsightMed.Application.Modules.LabParameters.Queries;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllLabParametersQueryHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILabParametersService> _mockLabParametersService;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly Mock<IOptions<InsightMed.Application.Options.MemoryCacheOptions>> _mockOptions;

    private readonly IMemoryCache _realMemoryCache;

    private readonly GetAllLabParametersQueryHandler _sut;

    public GetAllLabParametersQueryHandlerTests()
    {
        _mockMapper = new();
        _mockLabParametersService = new();
        _mockMemoryCache = new();
        _mockOptions = new();

        _realMemoryCache = new MemoryCache(new MemoryCacheOptions());

        _mockOptions
            .Setup(x => x.Value)
            .Returns(new InsightMed.Application.Options.MemoryCacheOptions() { AbsoluteExpirationMinutes = 5 });

        _sut = new(
            _mockMapper.Object,
            _mockLabParametersService.Object,
            _mockMemoryCache.Object,
            _mockOptions.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedData_WhenCacheIsHit()
    {
        // Arrange
        var query = new GetAllLabParametersQuery();
        string cacheKey = nameof(GetAllLabParametersQuery);

        var cachedResponse = new List<LabParameterResponse>
        {
            new()
            {
                Id = 1,
                Name = "Test"
            }
        };

        object? expectedValue = cachedResponse;

        _mockMemoryCache
            .Setup(c => c.TryGetValue(cacheKey, out expectedValue))
            .Returns(true);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.CachedResponse);
        Assert.Equal(cachedResponse, result.LabParameters);
        Assert.Single(result.LabParameters);
        Assert.Equal("Test", result.LabParameters[0].Name);
    }

    [Fact]
    public async Task Handle_ShouldFetchFreshData_WhenCacheIsMissed()
    {
        // Arrange
        var query = new GetAllLabParametersQuery();
        string cacheKey = nameof(GetAllLabParametersQuery);
        object expectedValue = null!;

        _mockMemoryCache
            .Setup(c => c.TryGetValue(cacheKey, out expectedValue!))
            .Returns(false);

        var serviceData = new List<LabParameter>
        {
            new() { Id = 2, Name = "Test parameter" }
        };

        _mockLabParametersService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(serviceData);

        var mappedData = new List<LabParameterResponse>
        {
            new() { Id = 2, Name = "Test parameter" }
        };

        _mockMapper
            .Setup(m => m.Map<List<LabParameterResponse>>(serviceData))
            .Returns(mappedData);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(c => c.CreateEntry(cacheKey))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.CachedResponse);
        mockCacheEntry.VerifySet(e => e.Value = mappedData, Times.Once);
        mockCacheEntry.VerifySet(e => e.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan>(), Times.Once);
        mockCacheEntry.VerifySet(e => e.Priority = CacheItemPriority.High, Times.Once);
        mockCacheEntry.Verify(e => e.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldReadCacheResponse_OnConsequentCalls()
    {
        // Arrange
        var query = new GetAllLabParametersQuery();

        var _sutWithRealCache = new GetAllLabParametersQueryHandler(
            _mockMapper.Object,
            _mockLabParametersService.Object,
            _realMemoryCache,
            _mockOptions.Object);

        var serviceData = new List<LabParameter>
        {
            new() { Id = 2, Name = "Test parameter" }
        };

        _mockLabParametersService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(serviceData);

        var mappedData = new List<LabParameterResponse>
        {
            new() { Id = 2, Name = "Test parameter" }
        };

        _mockMapper
            .Setup(m => m.Map<List<LabParameterResponse>>(serviceData))
            .Returns(mappedData);

        // Act
        var firstResult = await _sutWithRealCache.Handle(query, CancellationToken.None);
        var secondResult = await _sutWithRealCache.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(firstResult.CachedResponse);
        Assert.True(secondResult.CachedResponse);

        _mockLabParametersService.Verify(x => x.GetAllAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<LabParameterResponse>>(It.IsAny<object>()), Times.Once);
    }
}