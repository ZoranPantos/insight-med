using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Exceptions;
using MediatR;

namespace InsightMed.Application.Auth.Commands;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        string? userId = _currentUserService.GetUserId();

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User not found");

        await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
    }
}