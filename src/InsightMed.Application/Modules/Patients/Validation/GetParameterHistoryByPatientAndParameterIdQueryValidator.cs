using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Patients.Queries;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.Modules.Patients.Validation;

public class GetParameterHistoryByPatientAndParameterIdQueryValidator
    : AbstractValidator<GetParameterHistoryByPatientAndParameterIdQuery>
{
    private readonly IAppDbContext _context;

    public GetParameterHistoryByPatientAndParameterIdQueryValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(query => query.PatientId)
            .GreaterThan(0)
            .MustAsync(ExistInPatientsAsync)
            .WithMessage("Patient with ID {PropertyValue} not found");

        RuleFor(query => query.ParameterId)
            .GreaterThan(0)
            .MustAsync(ExistInLabParametersAsync)
            .WithMessage("Lab parameter with ID {PropertyValue} not found");
    }

    private async Task<bool> ExistInPatientsAsync(int patientId, CancellationToken cancellationToken)
    {
        bool exists = await _context.Patients
            .AnyAsync(patient => patient.Id == patientId, cancellationToken);

        return exists;
    }

    private async Task<bool> ExistInLabParametersAsync(int parameterId, CancellationToken cancellationToken)
    {
        bool exists = await _context.LabParameters
            .AnyAsync(labParameter => labParameter.Id == parameterId, cancellationToken);

        return exists;
    }
}