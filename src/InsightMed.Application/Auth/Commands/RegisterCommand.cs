using InsightMed.Application.Auth.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Auth.Commands;

public sealed record RegisterCommand(string Email, string Password) : IRequest;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService) =>
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        await _authService.RegisterAsync(request.Email, request.Password);
}