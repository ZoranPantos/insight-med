using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Patients.Commands;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.Modules.Patients.Validation;

public sealed class AddPatientCommandValidator : AbstractValidator<AddPatientCommand>
{
    private readonly IAppDbContext _context;

    public AddPatientCommandValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(command => command)
            .NotEmpty();

        RuleFor(command => command.Input.FirstName)
            .NotEmpty()
            .WithMessage("First name must not be empty");

        RuleFor(command => command.Input.LastName)
            .NotEmpty()
            .WithMessage("Last name must not be empty");

        RuleFor(command => command.Input.Phone)
            .NotEmpty()
            .WithMessage("Phone must not be empty");

        RuleFor(command => command.Input.Email)
            .NotEmpty()
            .WithMessage("Email must not be empty");

        RuleFor(command => command.Input.DateOfBirth)
            .NotEmpty()
            .Must(BeOlderThanUtcNow)
            .WithMessage("Date of birth must be older than now");

        RuleFor(command => command.Input.Uid)
            .NotEmpty()
            .MustAsync(BeUniqueAsync)
            .WithMessage("Patient with UID {PropertyValue} already exists");
    }

    private bool BeOlderThanUtcNow(DateOnly dateOfBirthUtc) =>
        dateOfBirthUtc < DateOnly.FromDateTime(DateTime.UtcNow);

    private async Task<bool> BeUniqueAsync(string uid, CancellationToken cancellationToken)
    {
        bool exists = await _context.Patients
            .AnyAsync(patient => patient.Uid == uid, cancellationToken);

        return !exists;
    }
}
