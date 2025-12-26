using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.LabParameters.Services;

public sealed class LabParametersService : ILabParametersService
{
    private readonly IAppDbContext _context;

    public LabParametersService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<LabParameter>> GetAllAsync()
    {
        return await _context.LabParameters
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
    }
}