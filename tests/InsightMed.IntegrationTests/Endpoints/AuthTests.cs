using System.Net;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class AuthTests : BaseIntegrationTest
{
    public AuthTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UnauthorizedCalls_ShouldNotYieldResults()
    {
        // Arrange
        int entityId = 1;

        // Act
        var response = await client.GetAsync($"api/patients/{entityId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}