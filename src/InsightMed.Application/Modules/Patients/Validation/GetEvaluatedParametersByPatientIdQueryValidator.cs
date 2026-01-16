using FluentValidation;
using InsightMed.Application.Modules.Patients.Queries;

namespace InsightMed.Application.Modules.Patients.Validation;

public sealed class GetEvaluatedParametersByPatientIdQueryValidator
    : AbstractValidator<GetEvaluatedParametersByPatientIdQuery>
{
    public GetEvaluatedParametersByPatientIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}