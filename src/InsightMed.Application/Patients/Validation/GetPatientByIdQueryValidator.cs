using FluentValidation;
using InsightMed.Application.Patients.Queries;

namespace InsightMed.Application.Patients.Validation;

public sealed class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
