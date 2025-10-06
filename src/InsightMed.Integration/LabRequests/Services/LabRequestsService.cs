using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.LabRequests.Services;

public sealed class LabRequestsService : ILabRequestsService
{
    private readonly IAppDbContext _context;

    public LabRequestsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<LabRequest>> GetAllAsync()
    {
        return await _context.LabRequests
            .AsNoTracking()
            .Include(labRequest => labRequest.Patient)
            .Include(labRequest => labRequest.LabReport)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
