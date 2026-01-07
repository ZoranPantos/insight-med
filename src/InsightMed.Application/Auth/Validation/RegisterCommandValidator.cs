using FluentValidation;
using InsightMed.Application.Auth.Commands;

namespace InsightMed.Application.Auth.Validation;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command)
            .NotEmpty();

        RuleFor(command => command.Email)
            .NotEmpty();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}