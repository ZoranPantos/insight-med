using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.LabReports.Services;

public sealed class LabReportsService : ILabReportsService
{
    private readonly IAppDbContext _context;

    public LabReportsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<(List<LabReport> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var items = await _context.LabReports
            .AsNoTracking()
            .Include(labReport => labReport.Patient)
            .OrderByDescending(labReport => labReport.Created)
            .ThenByDescending(labReport => labReport.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        int totalCount = await _context.LabReports
            .CountAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<List<LabReport>> GetAllByPatientIdAsync(int patientId)
    {
        return await _context.LabReports
            .AsNoTracking()
            .Include(LabReport => LabReport.Patient)
            .Where(labReport => labReport.PatientId == patientId)
            .OrderByDescending(labReport => labReport.Created)
            .ThenByDescending(labReport => labReport.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<LabReport?> GetByIdAsync(int id)
    {
        return await _context.LabReports
            .AsNoTracking()
            .Include(labReport => labReport.Patient)
            .FirstOrDefaultAsync(labReport => labReport.Id == id)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(LabReport labReport)
    {
        _context.LabReports.Add(labReport);

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task<(List<LabReport> Items, int TotalCount)> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize)
    {
        var query = _context.LabReports
            .AsNoTracking()
            .Include(r => r.Patient)
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
            .OrderByDescending(labReport => labReport.Created)
            .ThenByDescending(labReport => labReport.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }
}