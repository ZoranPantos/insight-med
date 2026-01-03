using InsightMed.Application.Auth.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<string>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService) =>
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        await _authService.LoginAsync(request.Email, request.Password);
}