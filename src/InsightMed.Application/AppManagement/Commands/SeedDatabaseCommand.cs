using InsightMed.Application.AppManagement.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.AppManagement.Commands;

public record SeedDatabaseCommand : IRequest;

public class SeedDatabaseCommandHandler : IRequestHandler<SeedDatabaseCommand>
{
    private readonly IDatabaseManagementService _databaseManagementService;

    public SeedDatabaseCommandHandler(IDatabaseManagementService databaseManagementService) =>
        _databaseManagementService = databaseManagementService ?? throw new ArgumentNullException(nameof(databaseManagementService));

    public async Task Handle(SeedDatabaseCommand request, CancellationToken cancellationToken)
    {
        await _databaseManagementService.Seed();
    }
}