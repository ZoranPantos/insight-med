using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.LabRequests.Services;

public sealed class LabRequestsService : ILabRequestsService
{
    private readonly IAppDbContext _context;

    public LabRequestsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<(List<LabRequest> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var items = await _context.LabRequests
            .AsNoTracking()
            .Include(labRequest => labRequest.Patient)
            .Include(labRequest => labRequest.LabReport)
            .OrderByDescending(labRequest => labRequest.Created)
            .ThenByDescending(labRequest => labRequest.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        int totalCount = await _context.LabRequests
            .CountAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
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

    public async Task<(List<LabRequest> Items, int TotalCount)> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize)
    {
        var query = _context.LabRequests
            .AsNoTracking()
            .Include(labRequest => labRequest.Patient)
            .Include(labRequest => labRequest.LabReport)
            .AsQueryable();

        foreach (string token in tokens)
        {
            string searchTerm = token.Trim().ToLower();

            query = query.Where(r =>
                r.Patient.FirstName.ToLower().Contains(searchTerm) ||
                r.Patient.LastName.ToLower().Contains(searchTerm) ||
                r.Patient.Uid.ToLower().Contains(searchTerm));
        }

        int totalCount = await query
            .CountAsync()
            .ConfigureAwait(false);

        var items = await query
            .OrderByDescending(labRequest => labRequest.Created)
            .ThenByDescending(labRequest => labRequest.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }
}