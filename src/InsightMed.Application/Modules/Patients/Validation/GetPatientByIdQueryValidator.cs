using FluentValidation;
using InsightMed.Application.Modules.Patients.Queries;

namespace InsightMed.Application.Modules.Patients.Validation;

public sealed class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}