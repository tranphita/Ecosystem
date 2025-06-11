using FluentValidation;
using MediatR;
using Ecosystem.Shared;

namespace Ecosystem.Application.Behaviors;

/// <summary>
/// Pipeline behavior để validate tất cả các requests qua FluentValidation.
/// </summary>
/// <typeparam name="TRequest">Loại request.</typeparam>
/// <typeparam name="TResponse">Loại response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => new Error(f.PropertyName, f.ErrorMessage)).ToArray();

            // Để đơn giản, chúng ta sẽ ném ValidationException
            // Trong thực tế, bạn có thể muốn return Result.Failure với validation errors
            throw new ValidationException(failures);
        }

        return await next();
    }
}