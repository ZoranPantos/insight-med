using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.LabRequests.Mapping;
using InsightMed.Application.Modules.LabRequests.Queries;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using Microsoft.Extensions.Options;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllLabRequestsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILabRequestsService> _mockLabRequestsService;
    private readonly Mock<ILabParametersService> _mockLabParametersService;
    private readonly Mock<IOptions<PagingOptions>> _mockPagingOptions;

    private readonly GetAllLabRequestsQueryHandler _sut;

    public GetAllLabRequestsQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LabRequestMappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _mockLabRequestsService = new();
        _mockLabParametersService = new();
        _mockPagingOptions = new();

        _mockPagingOptions
            .Setup(x => x.Value)
            .Returns(new PagingOptions { RequestsPageSize = 10 });

        _sut = new(
            _mapper,
            _mockLabRequestsService.Object,
            _mockLabParametersService.Object,
            _mockPagingOptions.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ShouldCallGetAllPaged_WhenSearchKeyIsNullOrWhitespace(string? searchKey)
    {
        // Arrange
        var query = new GetAllLabRequestsQuery(SearchKey: searchKey, PageNumber: 1);

        var labRequests = new List<LabRequest>
        {
            new()
            {
                Id = 1,
                Created = new DateTime(2026, 1, 22),
                LabRequestState = LabRequestState.Pending,
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                },
                LabParameterIds = [1, 2]
            }
        };

        var labParameters = new List<LabParameter>
        {
            new() { Id = 1, Name = "Hemoglobin" },
            new() { Id = 2, Name = "Glucose" }
        };

        _mockLabRequestsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labRequests, 1));

        _mockLabParametersService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(labParameters);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockLabRequestsService.Verify(
            x => x.GetAllPagedAsync(1, 10),
            Times.Once);

        _mockLabRequestsService.Verify(
            x => x.SearchByTokensPagedAsync(It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSearchByTokens_WhenSearchKeyIsProvided()
    {
        // Arrange
        var query = new GetAllLabRequestsQuery(SearchKey: "John Doe", PageNumber: 1);

        var labRequests = new List<LabRequest>
        {
            new()
            {
                Id = 5,
                Created = new DateTime(2026, 1, 23),
                LabRequestState = LabRequestState.Pending,
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                },
                LabParameterIds = [3]
            }
        };

        var labParameters = new List<LabParameter>
        {
            new() { Id = 3, Name = "Cholesterol" }
        };

        _mockLabRequestsService
            .Setup(x => x.SearchByTokensPagedAsync(
                It.Is<string[]>(tokens => tokens.Length == 2 && tokens[0].Equals("John") && tokens[1].Equals("Doe")),
                1,
                10))
            .ReturnsAsync((labRequests, 1));

        _mockLabParametersService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(labParameters);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.LabRequests);
        Assert.Equal(5, result.LabRequests[0].Id);
        Assert.Equal("John Doe", result.LabRequests[0].PatientFullName);
        Assert.Single(result.LabRequests[0].LabParameters);
        Assert.Equal("Cholesterol", result.LabRequests[0].LabParameters[0].Name);

        _mockLabRequestsService.Verify(
            x => x.SearchByTokensPagedAsync(
                It.Is<string[]>(tokens => tokens.Length == 2 && tokens[0].Equals("John") && tokens[1].Equals("Doe")),
                1,
                10),
            Times.Once);

        _mockLabRequestsService.Verify(
            x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMapLabReportId_WhenRequestIsCompleted()
    {
        // Arrange
        var query = new GetAllLabRequestsQuery(SearchKey: null, PageNumber: 1);

        var labRequests = new List<LabRequest>
        {
            new()
            {
                Id = 1,
                Created = DateTime.UtcNow,
                LabRequestState = LabRequestState.Completed,
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Uid = "P456"
                },
                LabReport = new LabReport { Id = 100 },
                LabParameterIds = []
            }
        };

        var labParameters = new List<LabParameter>();

        _mockLabRequestsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labRequests, 1));

        _mockLabParametersService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(labParameters);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.LabRequests);
        Assert.Equal(100, result.LabRequests[0].LabReportId);
        Assert.Equal(LabRequestState.Completed, result.LabRequests[0].LabRequestState);
    }

    [Fact]
    public async Task Handle_ShouldSkipMissingLabParameters()
    {
        // Arrange
        var query = new GetAllLabRequestsQuery(SearchKey: null, PageNumber: 1);

        var labRequests = new List<LabRequest>
        {
            new()
            {
                Id = 1,
                Created = DateTime.UtcNow,
                LabRequestState = LabRequestState.Pending,
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Uid = "P001"
                },
                LabParameterIds = [1, 999, 2]
            }
        };

        var labParameters = new List<LabParameter>
        {
            new() { Id = 1, Name = "Hemoglobin" },
            new() { Id = 2, Name = "Glucose" }
        };

        _mockLabRequestsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labRequests, 1));

        _mockLabParametersService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(labParameters);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.LabRequests);
        Assert.Equal(2, result.LabRequests[0].LabParameters.Count);
        Assert.Equal("Hemoglobin", result.LabRequests[0].LabParameters[0].Name);
        Assert.Equal("Glucose", result.LabRequests[0].LabParameters[1].Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task Handle_ShouldDefaultToPageOne_WhenPageNumberIsZeroOrNegative(int pageNumber)
    {
        // Arrange
        var query = new GetAllLabRequestsQuery(SearchKey: null, PageNumber: pageNumber);

        var labRequests = new List<LabRequest>();
        var labParameters = new List<LabParameter>();

        _mockLabRequestsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labRequests, 0));

        _mockLabParametersService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(labParameters);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.PageNumber);

        _mockLabRequestsService.Verify(
            x => x.GetAllPagedAsync(1, 10),
            Times.Once);
    }
}