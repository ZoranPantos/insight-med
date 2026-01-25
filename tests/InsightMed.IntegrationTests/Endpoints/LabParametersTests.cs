using InsightMed.Application.Modules.LabParameters.Models;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class LabParametersTests : BaseIntegrationTest, IAsyncLifetime
{
    public LabParametersTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync()
    {
        await SeedAsync();
        await AuthenticateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetEndpoint_ShouldRetrieveExistingLabParameters()
    {
        // Act
        var response = await client.GetAsync("api/LabParameters", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var labParametersData = await response.Content
            .ReadFromJsonAsync<GetAllLabParametersQueryResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(labParametersData);
        Assert.NotNull(labParametersData.LabParameters);
        Assert.NotEmpty(labParametersData.LabParameters);
    }
}