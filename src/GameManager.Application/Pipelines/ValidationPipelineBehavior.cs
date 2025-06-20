using System.Diagnostics;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;

namespace GameManager.Application.Pipelines;

public class ValidationPipelineBehavior<TRequest, TResponse>
: IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (typeof(TResponse).IsAssignableTo(typeof(IError<ApplicationError>)))
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            if (validationResults.Any(v => !v.IsValid))
            {
                return (dynamic) ApplicationError.Validation<TRequest>(validationResults.First(v => !v.IsValid));
            }
        }

        var response = await next();

        return response;
    }
}