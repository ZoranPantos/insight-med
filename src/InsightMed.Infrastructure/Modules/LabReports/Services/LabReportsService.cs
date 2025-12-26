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

    public async Task<List<LabReport>> GetAllAsync()
    {
        return await _context.LabReports
            .AsNoTracking()
            .Include(labReport => labReport.Patient)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<LabReport>> GetAllByPatientIdAsync(int patientId)
    {
        return await _context.LabReports
            .AsNoTracking()
            .Include(LabReport => LabReport.Patient)
            .Where(labReport => labReport.PatientId == patientId)
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
}
