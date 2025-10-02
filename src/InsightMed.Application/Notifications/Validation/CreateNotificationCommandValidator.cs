using FluentValidation;
using InsightMed.Application.Notifications.Commands;

namespace InsightMed.Application.Notifications.Validation;

public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    // TODO: Inject db context to valide ID

    public CreateNotificationCommandValidator()
    {
        RuleFor(command => command.Message).NotEmpty();
        RuleFor(command => command.LabReportId).GreaterThan(0);
    }
}
