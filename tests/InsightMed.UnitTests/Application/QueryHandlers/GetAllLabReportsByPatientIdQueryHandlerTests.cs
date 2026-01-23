using AutoMapper;
using InsightMed.Application.Modules.LabReports.Mapping;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllLabReportsByPatientIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILabReportsService> _mockLabReportsService;

    private readonly GetAllLabReportsByPatientIdQueryHandler _sut;

    public GetAllLabReportsByPatientIdQueryHandlerTests()
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
    public async Task Handle_ShouldMapCorrectly()
    {
        // Arrange
        int patientId = 1;
        var query = new GetAllLabReportsByPatientIdQuery(patientId);

        var labReports = new List<LabReport>
        {
            new()
            {
                Id = 1,
                Content = "Test Content 1",
                Created = new DateTime(2026, 1, 22, 14, 30, 0),
                PatientId = patientId,
                Patient = new Patient
                {
                    Id = patientId,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                }
            },
            new()
            {
                Id = 2,
                Content = "Test Content 2",
                Created = new DateTime(2026, 1, 23, 10, 15, 0),
                PatientId = patientId,
                Patient = new Patient
                {
                    Id = patientId,
                    FirstName = "John",
                    LastName = "Doe",
                    Uid = "P123"
                }
            }
        };

        _mockLabReportsService
            .Setup(x => x.GetAllByPatientIdAsync(patientId))
            .ReturnsAsync(labReports);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.LabReports);
        Assert.Equal(2, result.LabReports.Count);

        Assert.Equal(1, result.LabReports[0].Id);
        Assert.Equal(new DateTime(2026, 1, 22, 14, 30, 0), result.LabReports[0].Created);
        Assert.Equal("John Doe", result.LabReports[0].PatientFullName);
        Assert.Equal("P123", result.LabReports[0].PatientUid);

        Assert.Equal(2, result.LabReports[1].Id);
        Assert.Equal(new DateTime(2026, 1, 23, 10, 15, 0), result.LabReports[1].Created);
        Assert.Equal("John Doe", result.LabReports[1].PatientFullName);
        Assert.Equal("P123", result.LabReports[1].PatientUid);

        _mockLabReportsService.Verify(
            x => x.GetAllByPatientIdAsync(patientId),
            Times.Once);
    }
}