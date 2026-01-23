using AutoMapper;
using InsightMed.Application.Modules.LabReports.Mapping;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllLabReportsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILabReportsService> _mockLabReportsService;
    private readonly Mock<IOptions<PagingOptions>> _mockPagingOptions;

    private readonly GetAllLabReportsQueryHandler _sut;

    public GetAllLabReportsQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LabReportMappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _mockLabReportsService = new();
        _mockPagingOptions = new();

        _mockPagingOptions
            .Setup(x => x.Value)
            .Returns(new PagingOptions { ReportsPageSize = 10 });

        _sut = new(
            _mapper,
            _mockLabReportsService.Object,
            _mockPagingOptions.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task Handle_ShouldCallGetAllPaged_WhenSearchKeyIsNullOrWhitespace(string? searchKey)
    {
        // Arrange
        var query = new GetAllLabReportsQuery(SearchKey: searchKey, PageNumber: 1);

        var labReports = new List<LabReport>
        {
            new()
            {
                Id = 1,
                Created = new DateTime(2026, 1, 22),
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                }
            },
            new()
            {
                Id = 2,
                Created = new DateTime(2026, 1, 23),
                PatientId = 2,
                Patient = new Patient
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Uid = "P456"
                }
            }
        };

        _mockLabReportsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labReports, 2));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.LabReports.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);

        Assert.Equal(1, result.LabReports[0].Id);
        Assert.Equal("John Doe", result.LabReports[0].PatientFullName);
        Assert.Equal("P123", result.LabReports[0].PatientUid);

        Assert.Equal(2, result.LabReports[1].Id);
        Assert.Equal("Jane Smith", result.LabReports[1].PatientFullName);
        Assert.Equal("P456", result.LabReports[1].PatientUid);

        _mockLabReportsService.Verify(
            x => x.GetAllPagedAsync(1, 10),
            Times.Once);

        _mockLabReportsService.Verify(
            x => x.SearchByTokensPagedAsync(It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public async Task Handle_ShouldDefaultToPageOne_WhenPageNumberIsZeroOrNegative(int pageNumber)
    {
        // Arrange
        var query = new GetAllLabReportsQuery(SearchKey: null, PageNumber: pageNumber);

        var labReports = new List<LabReport>();

        _mockLabReportsService
            .Setup(x => x.GetAllPagedAsync(1, 10))
            .ReturnsAsync((labReports, 0));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.PageNumber);

        _mockLabReportsService.Verify(
            x => x.GetAllPagedAsync(1, 10),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSearchByTokens_WhenSearchKeyIsProvided()
    {
        // Arrange
        var query = new GetAllLabReportsQuery(SearchKey: "John Doe", PageNumber: 1);

        var labReports = new List<LabReport>
        {
            new()
            {
                Id = 1,
                Created = new DateTime(2026, 1, 22),
                PatientId = 1,
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                }
            }
        };

        _mockLabReportsService
            .Setup(x => x.SearchByTokensPagedAsync(
                It.Is<string[]>(tokens => tokens.Length == 2 && tokens[0].Equals("John") && tokens[1].Equals("Doe")),
                1,
                10))
            .ReturnsAsync((labReports, 1));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.LabReports);
        Assert.Equal(1, result.LabReports[0].Id);
        Assert.Equal("John Doe", result.LabReports[0].PatientFullName);
        Assert.Equal(1, result.TotalCount);

        _mockLabReportsService.Verify(
            x => x.SearchByTokensPagedAsync(
                It.Is<string[]>(tokens => tokens.Length == 2 && tokens[0].Equals("John") && tokens[1].Equals("Doe")),
                1,
                10),
            Times.Once);

        _mockLabReportsService.Verify(
            x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }
}