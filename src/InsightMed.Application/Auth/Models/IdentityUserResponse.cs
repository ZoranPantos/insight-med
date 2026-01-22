namespace InsightMed.Application.Auth.Models;

public sealed class IdentityUserResponse
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTime PasswordLastChanged { get; set; }
}