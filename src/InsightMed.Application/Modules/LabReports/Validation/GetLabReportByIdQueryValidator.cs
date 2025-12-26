using FluentValidation;
using InsightMed.Application.Modules.LabReports.Queries;

namespace InsightMed.Application.Modules.LabReports.Validation;

public sealed class GetLabReportByIdQueryValidator : AbstractValidator<GetLabReportByIdQuery>
{
    public GetLabReportByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
