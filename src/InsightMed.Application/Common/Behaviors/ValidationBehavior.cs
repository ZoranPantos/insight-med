using FluentValidation;
using InsightMed.Application.Common.Exceptions;
using MediatR;

namespace InsightMed.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults =
                await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)))
                .ConfigureAwait(false);

            var failures = validationResults
                .Where(result => result.Errors.Count > 0)
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Count > 0)
            {
                string message = string.Join(
                    Environment.NewLine,
                    failures.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));

                throw new InvalidClientDataException(message);
            }
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
