using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Patients.Commands;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.Modules.Patients.Validation;

public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    private readonly IAppDbContext _context;

    public UpdatePatientCommandValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(command => command)
            .NotEmpty();

        RuleFor(command => command.Input.HeightCm)
            .GreaterThan(0)
            .WithMessage("Height value is invalid")
            .LessThan(300)
            .WithMessage("Height value is invalid");

        RuleFor(command => command.Input.WeightKg)
            .GreaterThan(0)
            .WithMessage("Weight value is invalid")
            .LessThan(700)
            .WithMessage("Weight value is invalid");

        RuleFor(command => command.Input.Id)
            .MustAsync(ExistAsync)
            .WithMessage("Patient with ID {PropertyValue} not found");
    }

    private async Task<bool> ExistAsync(int id, CancellationToken cancellationToken)
    {
        bool exists = await _context.Patients
            .AnyAsync(patient => patient.Id == id, cancellationToken);

        return exists;
    }
}