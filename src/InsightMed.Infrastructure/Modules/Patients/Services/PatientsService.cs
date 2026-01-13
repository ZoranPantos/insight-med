using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.Patients.Services;

public class PatientsService : IPatientsService
{
    private readonly IAppDbContext _context;

    public PatientsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));
    
    public async Task<(List<Patient> Items, int TotalCount)> GetAllAsync()
    {
        var items = await _context.Patients
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

        int totalCount = await _context.Patients
            .CountAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<(List<Patient> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var items = await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .ThenBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        int totalCount = await _context.Patients
            .CountAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients
            .AsNoTracking()
            .Include(patient => patient.LabReports)
            .Include(patient => patient.LabRequests)
            .ThenInclude(request => request.LabReport)
            .FirstOrDefaultAsync(patient => patient.Id == id)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Patient patient)
    {
        await _context.Patients
            .AddAsync(patient)
            .ConfigureAwait(false);

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task<(List<Patient> Items, int TotalCount)> SearchByTokensPagedAsync(
        string[] tokens, int pageNumber, int pageSize)
    {
        var query = _context.Patients
            .AsNoTracking()
            .AsQueryable();

        foreach (string token in tokens)
        {
            string searchTerm = token.Trim().ToLower();

            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                p.Uid.ToLower().Contains(searchTerm));
        }

        int totalCount = await query
            .CountAsync()
            .ConfigureAwait(false);

        var items = await query
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .ThenBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }
}