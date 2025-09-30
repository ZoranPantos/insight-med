using FluentValidation;
using InsightMed.Application.Patients.Queries;

namespace InsightMed.Application.Patients.Validation;

public class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
