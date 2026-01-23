using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Mapping;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetLabReportByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILabReportsService> _mockLabReportsService;

    private readonly GetLabReportByIdQueryHandler _sut;

    public GetLabReportByIdQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LabReportMappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _mockLabReportsService = new();

        _sut = new(_mapper, _mockLabReportsService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrow_ForLabRequestNotFound()
    {
        // Arrange
        var query = new GetLabReportByIdQuery(Id: 999);

        _mockLabReportsService
            .Setup(x => x.GetByIdAsync(query.Id))
            .ReturnsAsync((LabReport?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _sut.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectly()
    {
        // Arrange
        var query = new GetLabReportByIdQuery(Id: 1);

        var labReport = new LabReport
        {
            Id = 1,
            Content = "Test lab report content with medical data",
            Created = new DateTime(2026, 1, 22, 14, 30, 0),
            PatientId = 5,
            Patient = new Patient
            {
                Id = 5,
                FirstName = "John",
                LastName = "Doe",
                Uid = "P123"
            }
        };

        _mockLabReportsService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(labReport);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test lab report content with medical data", result.Content);
        Assert.Equal(new DateTime(2026, 1, 22, 14, 30, 0), result.Created);
        Assert.Equal("John Doe", result.PatientFullName);
        Assert.Equal("P123", result.PatientUid);

        _mockLabReportsService.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);
    }
}