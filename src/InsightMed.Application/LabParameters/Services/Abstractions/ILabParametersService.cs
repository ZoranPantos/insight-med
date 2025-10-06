using InsightMed.Domain.Entities;

namespace InsightMed.Application.LabParameters.Services.Abstractions;

public interface ILabParametersService
{
    Task<List<LabParameter>> GetAllAsync();
}
