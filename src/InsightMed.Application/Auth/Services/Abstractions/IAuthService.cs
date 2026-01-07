using InsightMed.Application.Auth.Models;

namespace InsightMed.Application.Auth.Services.Abstractions;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);
    Task RegisterAsync(string email, string password);
    Task<IdentityUserResponse?> GetUserByIdAsync(string userId);
    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}