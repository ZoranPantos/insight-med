using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Patients.Services;

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
            .FirstOrDefaultAsync(patient => patient.Id == id)
            .ConfigureAwait(false);
    }
}
