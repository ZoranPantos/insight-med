using InsightMed.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly CustomWebApplicationFactory fixture;
    protected readonly AppDbContext context;
    protected readonly HttpClient client;

    private record LoginResponse(string Token);

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        fixture = factory;

        _scope = fixture.Services.CreateScope();
        context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        client = fixture.CreateClient();
    }

    /// <summary>
    /// Requires SeedAsync to be called beforehand if we are relying on a user from AspNetUsers table.
    /// </summary>
    /// <returns></returns>
    protected async Task AuthenticateAsync()
    {
        var loginModel = new
        {
            Email = "default@test.com",
            Password = "Default1!"
        };

        var loginResponse = await client.PostAsJsonAsync("api/auth/login", loginModel);
        loginResponse.EnsureSuccessStatusCode();

        var authData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authData!.Token);
    }

    protected async Task SeedAsync()
    {
        var seedResponse = await client.GetAsync("api/AppManagement/SeedData");
        seedResponse.EnsureSuccessStatusCode();
    }

    public void Dispose() => _scope?.Dispose();
}