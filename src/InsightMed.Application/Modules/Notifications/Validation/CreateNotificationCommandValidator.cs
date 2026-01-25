using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Notifications.Commands;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Application.Modules.Notifications.Validation;

public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    private readonly IAppDbContext _context;

    public CreateNotificationCommandValidator(IAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(command => command)
            .NotEmpty();

        RuleFor(command => command.Message)
            .NotEmpty();

        RuleFor(command => command.LabReportId)
            .GreaterThan(0)
            .MustAsync(LabReportExistsAsync)
            .WithMessage("Lab Report with ID {PropertyValue} not found");
    }

    private async Task<bool> LabReportExistsAsync(int labReportId, CancellationToken cancellationToken) =>
        await _context.LabReports.AnyAsync(labReport => labReport.Id == labReportId, cancellationToken);
}