using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.LabParameters.Services;

public sealed class LabParametersService : ILabParametersService
{
    private readonly IAppDbContext _context;

    public LabParametersService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<LabParameter>> GetAllAsync()
    {
        return await _context.LabParameters
            .AsNoTracking()
            .OrderBy(labParameter => labParameter.Name)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<LabParameter>> GetAllByPatientIdAsync(int id)
    {
        var labParameterIds = await _context.LabRequests
            .AsNoTracking()
            .Where(labRequest => labRequest.PatientId == id && labRequest.LabRequestState == LabRequestState.Completed)
            .SelectMany(labRequest => labRequest.LabParameterIds)
            .Distinct()
            .ToListAsync()
            .ConfigureAwait(false);

        return await _context.LabParameters
            .AsNoTracking()
            .Where(labParameter => labParameterIds.Contains(labParameter.Id))
            .OrderBy(labParameter => labParameter.Name)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}