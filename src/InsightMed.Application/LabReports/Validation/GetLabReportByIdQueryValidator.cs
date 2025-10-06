using FluentValidation;
using InsightMed.Application.LabReports.Queries;

namespace InsightMed.Application.LabReports.Validation;

public sealed class GetLabReportByIdQueryValidator : AbstractValidator<GetLabReportByIdQuery>
{
    public GetLabReportByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
