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
    
    public async Task<List<Patient>> GetAllAsync()
    {
        return await _context.Patients
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
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

    public async Task<List<Patient>> SearchByTokensAsync(string[] tokens)
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

        return await query
            .ToListAsync()
            .ConfigureAwait(false);
    }
}