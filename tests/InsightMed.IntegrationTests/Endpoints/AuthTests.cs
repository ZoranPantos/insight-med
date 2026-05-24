using InsightMed.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class AuthTests : BaseIntegrationTest, IAsyncLifetime
{
    private record LoginResponse(string Token);

    public AuthTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync() => await SeedAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

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

    [Fact]
    public async Task LoginEndpoint_ShouldIssueTokenWithExpirationFromConfiguration()
    {
        // Arrange
        var loginModel = new
        {
            Email = "default@test.com",
            Password = "Default1!"
        };

        using var scope = fixture.Services.CreateScope();
        var jwtOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        var callTimeUtc = DateTime.UtcNow;

        // Act
        var loginResponse = await client.PostAsJsonAsync(
            "api/auth/login",
            loginModel,
            TestContext.Current.CancellationToken);

        loginResponse.EnsureSuccessStatusCode();

        var authData = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(authData);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(authData.Token);
        var expectedExpiration = callTimeUtc.AddDays(jwtOptions.ExpiresInDays);

        // Allow a small tolerance for the time spent issuing the token
        var tolerance = TimeSpan.FromSeconds(30);
        Assert.InRange(
            jwt.ValidTo,
            expectedExpiration - tolerance,
            expectedExpiration + tolerance);
    }
}
