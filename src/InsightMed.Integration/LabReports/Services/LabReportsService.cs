using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.LabReports.Services;

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
}
