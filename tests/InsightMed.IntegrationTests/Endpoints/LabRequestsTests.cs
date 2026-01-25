using InsightMed.API.DTOs;
using InsightMed.Application.Modules.LabRequests.Models;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class LabRequestsTests : BaseIntegrationTest, IAsyncLifetime
{
    public LabRequestsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync()
    {
        await SeedAsync();
        await AuthenticateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetEndpoint_ShouldRetrieveExistingLabRequests()
    {
        // Act
        var response = await client.GetAsync("api/LabRequests", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var labRequestsData = await response.Content
            .ReadFromJsonAsync<GetAllLabRequestsQueryResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(labRequestsData);
        Assert.NotNull(labRequestsData.LabRequests);
        Assert.NotEmpty(labRequestsData.LabRequests);
    }

    [Fact]
    public async Task CreateEndpoint_ShouldCreateANewLabRequestInDatabase()
    {
        // Arrange
        var newLabRequest = new LabRequestInputModel
        {
            PatientId = 1,
            LabParameterIds = [1, 2]
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "api/LabRequests", newLabRequest, TestContext.Current.CancellationToken);

        // Assert
        var createdRequest = context.LabRequests
            .Where(lr => lr.PatientId == newLabRequest.PatientId)
            .OrderByDescending(lr => lr.Id)
            .FirstOrDefault();


        Assert.NotNull(createdRequest);
        Assert.Equal(newLabRequest.LabParameterIds.Count, createdRequest.LabParameterIds.Count);
    }
}