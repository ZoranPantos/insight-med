using FluentValidation;
using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.LabReports.Queries;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.LabReports.Validation;

public sealed class GetAllLabReportsByPatientIdQueryValidator : AbstractValidator<GetAllLabReportsByPatientIdQuery>
{
    private readonly IAppDbContext _context;

    public GetAllLabReportsByPatientIdQueryValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(query => query.PatientId)
            .GreaterThan(0)
            .MustAsync(PatientExistsAsync)
            .WithMessage("Patient with ID {PropertyValue} not found");
    }

    private async Task<bool> PatientExistsAsync(int patientId, CancellationToken cancellationToken) =>
        await _context.Patients.AnyAsync(patient => patient.Id == patientId, cancellationToken);
}
