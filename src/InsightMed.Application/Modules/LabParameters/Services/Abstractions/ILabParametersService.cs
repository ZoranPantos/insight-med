using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabParameters.Services.Abstractions;

public interface ILabParametersService
{
    Task<List<LabParameter>> GetAllAsync();
    Task<List<LabParameter>> GetAllByPatientIdAsync(int id);
}