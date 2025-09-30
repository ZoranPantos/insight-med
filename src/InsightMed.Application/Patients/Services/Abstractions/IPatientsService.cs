using InsightMed.Domain.Entities;

namespace InsightMed.Application.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<List<Patient>> GetAllPatients();
}
