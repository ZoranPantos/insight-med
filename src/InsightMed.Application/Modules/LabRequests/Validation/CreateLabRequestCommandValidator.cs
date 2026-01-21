using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.LabRequests.Commands;
using InsightMed.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.Modules.LabRequests.Validation;

public sealed class CreateLabRequestCommandValidator : AbstractValidator<CreateLabRequestCommand>
{
    private readonly IAppDbContext _context;

    public CreateLabRequestCommandValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(command => command)
            .NotEmpty();

        RuleFor(command => command.PatientId)
            .GreaterThan(0)
            .MustAsync(PatientExistsAsync)
            .WithMessage("Patient with ID {PropertyValue} not found");

        RuleFor(command => command.LabParameterIds)
            .NotEmpty()
            .Must(LabParameterIdsAreDistinct)
            .WithMessage("LabParameterIds must not contain duplicate values");

        RuleFor(command => command)
            .MustAsync(NoDuplicatePendingLabRequestAsync)
            .WithMessage("A pending lab request with the same patient and identical lab parameters already exists");
    }

    private async Task<bool> PatientExistsAsync(int patientId, CancellationToken cancellationToken) =>
        await _context.Patients.AnyAsync(patient => patient.Id == patientId, cancellationToken);

    private bool LabParameterIdsAreDistinct(List<int> ids) => ids.Distinct().Count() == ids.Count;

    private async Task<bool> NoDuplicatePendingLabRequestAsync(
        CreateLabRequestCommand command,
        CancellationToken cancellationToken)
    {
        var pendingRequests = await _context.LabRequests
            .Where(labRequest =>
                labRequest.PatientId == command.PatientId &&
                labRequest.LabRequestState == LabRequestState.Pending)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var newRequestIds = new HashSet<int>(command.LabParameterIds);

        foreach (var pendingRequest in pendingRequests)
        {
            if (pendingRequest.LabParameterIds is not null && newRequestIds.SetEquals(pendingRequest.LabParameterIds))
            {
                return false;
            }
        }

        return true;
    }
}