using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
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

    public async Task AddAsync(LabRequest labRequest)
    {
        _context.LabRequests.Add(labRequest);

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task<int?> SetStateAsync(int id, LabRequestState state)
    {
        var labRequest = await _context.LabRequests
            .FirstOrDefaultAsync(lr => lr.Id == id)
            .ConfigureAwait(false);

        if (labRequest is null) return null;

        labRequest.LabRequestState = state;

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return labRequest.Id;
    }
}