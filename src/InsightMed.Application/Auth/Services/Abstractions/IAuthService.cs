namespace InsightMed.Application.Auth.Services.Abstractions;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);
}