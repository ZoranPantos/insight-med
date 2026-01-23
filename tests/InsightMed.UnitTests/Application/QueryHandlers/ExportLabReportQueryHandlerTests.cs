using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class ExportLabReportQueryHandlerTests
{
    private readonly Mock<ILabReportsService> _mockLabReportsService;
    private readonly Mock<IPdfLabReportGeneratorService> _mockPdfLabReportGeneratorService;

    private readonly ExportLabReportQueryHandler _sut;

    public ExportLabReportQueryHandlerTests()
    {
        _mockLabReportsService = new();
        _mockPdfLabReportGeneratorService = new();

        _sut = new(_mockLabReportsService.Object, _mockPdfLabReportGeneratorService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrow_ForLabReportNotFound()
    {
        // Arrange
        var query = new ExportLabReportQuery(Id: 1);

        _mockLabReportsService
            .Setup(x => x.GetByIdAsync(query.Id))
            .ReturnsAsync((LabReport?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _sut.Handle(query, CancellationToken.None));

        _mockPdfLabReportGeneratorService.Verify(
            x => x.GenerateLabReportPdf(It.IsAny<LabReport>(), It.IsAny<List<ReportItemDto>>()),
            Times.Never());
    }
}