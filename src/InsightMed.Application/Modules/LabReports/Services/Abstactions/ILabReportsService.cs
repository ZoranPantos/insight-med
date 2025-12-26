using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabReports.Services.Abstactions;

public interface ILabReportsService
{
    Task<List<LabReport>> GetAllAsync();
    Task<List<LabReport>> GetAllByPatientIdAsync(int patientId);
    Task<LabReport?> GetByIdAsync(int id);
    Task AddAsync(LabReport labReport);
}