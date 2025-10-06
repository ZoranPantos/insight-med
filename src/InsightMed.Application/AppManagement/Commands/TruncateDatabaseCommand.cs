using InsightMed.Application.AppManagement.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.AppManagement.Commands;

public sealed record TruncateDatabaseCommand : IRequest;

public sealed class TruncateDatabaseCommandHandler : IRequestHandler<TruncateDatabaseCommand>
{
    private readonly IDatabaseManagementService _databaseManagementService;

    public TruncateDatabaseCommandHandler(IDatabaseManagementService databaseManagementService) =>
        _databaseManagementService = databaseManagementService ?? throw new ArgumentNullException(nameof(databaseManagementService));

    public async Task Handle(TruncateDatabaseCommand request, CancellationToken cancellationToken) =>
        await _databaseManagementService.TruncateAsync();
}
