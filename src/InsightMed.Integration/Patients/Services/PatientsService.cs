using InsightMed.Application.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Integration.Data;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Patients.Services;

public class PatientsService : IPatientsService
{
    private readonly AppDbContext _context;

    public PatientsService(AppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _context.Patients
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
