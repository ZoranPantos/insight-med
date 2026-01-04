namespace InsightMed.Application.Auth.Services.Abstractions;

public interface ICurrentUserService
{
    string? GetUserId();
}