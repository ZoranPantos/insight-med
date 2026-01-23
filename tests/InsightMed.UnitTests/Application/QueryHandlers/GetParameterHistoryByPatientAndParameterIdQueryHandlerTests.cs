using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Domain.Entities;
using Moq;
using System.Text.Json;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetParameterHistoryByPatientAndParameterIdQueryHandlerTests
{
    private readonly Mock<ILabReportsService> _mockLabReportsService;
    private readonly GetParameterHistoryByPatientAndParameterIdQueryHandler _sut;

    public GetParameterHistoryByPatientAndParameterIdQueryHandlerTests()
    {
        _mockLabReportsService = new Mock<ILabReportsService>();
        _sut = new(_mockLabReportsService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContentIsInvalidJson()
    {
        // Arrange
        int patientId = 1;
        int parameterId = 100;
        var query = new GetParameterHistoryByPatientAndParameterIdQuery(patientId, parameterId);

        string invalidJsonContent = $@"{{ ""Id"":{parameterId}, ... BROKEN JSON ... }}";

        var labReports = new List<LabReport>
        {
            new()
            {
                Id = 1,
                Content = invalidJsonContent,
                Created = DateTime.UtcNow
            }
        };

        _mockLabReportsService
            .Setup(x => x.GetAllByPatientIdAsync(patientId))
            .ReturnsAsync(labReports);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _sut.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectlyMappedAndSortedResponse_WhenDataExists()
    {
        // Arrange
        int patientId = 123;
        int targetParamId = 55;

        var dateOld = new DateTime(2023, 1, 10);
        var dateNew = new DateTime(2023, 2, 20);

        var commonReference = new ReferenceRange
        {
            Unit = "mg/dL",
            MinThreshold = 70,
            MaxThreshold = 100
        };

        var contentItem1 = new ContentItem
        {
            Id = targetParamId,
            Name = "Glucose",
            Measurement = 95.5,
            IsPositive = false,
            Reference = commonReference
        };

        var contentItem2 = new ContentItem
        {
            Id = targetParamId,
            Name = "Glucose",
            Measurement = 105.0,
            IsPositive = true,
            Reference = commonReference
        };

        var reportsFromDb = new List<LabReport>
        {
            new() {
                Id = 1,
                Created = dateNew,
                Content = JsonSerializer.Serialize(new List<ContentItem> { contentItem2 })
            },
            new() {
                Id = 2,
                Created = dateOld,
                Content = JsonSerializer.Serialize(new List<ContentItem> { contentItem1 })
            },
            new() {
                Id = 3,
                Created = DateTime.Now,
                Content = JsonSerializer.Serialize(new List<ContentItem> { new ContentItem { Id = 999 } })
            }
        };

        _mockLabReportsService
            .Setup(x => x.GetAllByPatientIdAsync(patientId))
            .ReturnsAsync(reportsFromDb);

        var query = new GetParameterHistoryByPatientAndParameterIdQuery(patientId, targetParamId);

        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(targetParamId, result.Id);
        Assert.Equal("Glucose", result.Name);
        Assert.Equal("mg/dL", result.Unit);

        Assert.NotNull(result.LabParameterReference);
        Assert.Equal(70, result.LabParameterReference.MinThreshold);
        Assert.Equal(100, result.LabParameterReference.MaxThreshold);

        Assert.Equal(2, result.History.Count);

        var firstRecord = result.History[0];
        Assert.Equal(dateOld, firstRecord.Created);
        Assert.Equal(95.5, firstRecord.Measurement);

        var secondRecord = result.History[1];
        Assert.Equal(dateNew, secondRecord.Created);
        Assert.Equal(105.0, secondRecord.Measurement);
    }
}