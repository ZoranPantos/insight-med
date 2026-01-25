using InsightMed.Application.Modules.LabReports.Models;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class LabReportsTests : BaseIntegrationTest, IAsyncLifetime
{
    public LabReportsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync()
    {
        await SeedAsync();
        await AuthenticateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetEndpoint_ShouldRetrieveExistingLabReport()
    {
        // Arrange
        int reportId = 1;

        // Act
        var response = await client.GetAsync($"api/LabReports/{reportId}", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var report = await response.Content.ReadFromJsonAsync<GetLabReportByIdQueryResponse>(TestContext.Current.CancellationToken);
        
        // Assert
        Assert.NotNull(report);
        Assert.Equal(reportId, report.Id);
    }

    [Fact]
    public async Task ExportEndpoint_ShouldGenerateLabReportPdf()
    {
        // Arrange
        int reportId = 1;

        // Act
        var response = await client.GetAsync($"api/LabReports/{reportId}/export", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

        var fileBytes = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(fileBytes);
        Assert.True(fileBytes.Length > 0, "The exported PDF should not be an empty file.");

        var fileName = response.Content.Headers.ContentDisposition?.FileName;
        Assert.NotNull(fileName);
    }
}